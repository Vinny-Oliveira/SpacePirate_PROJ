
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Rendering;

namespace Clayxels{
	[ExecuteInEditMode]
	public class Claymation : MonoBehaviour{
		public TextAsset claymationFile = null;
		public Material material = null;
		public int frameRate = 30;
		public bool playAnim = false;
		
		bool loaded = false;
		int fileFormat = 0;
		float voxelSize = 0.1f;
		float splatRadius = 0.1f;
		Bounds renderBounds = new Bounds();
		Vector3 boundsScale;
		int numFrames = 0;
		int numChunks = 0;
		int chunksX = 0;
		int chunksY = 0;
		int chunksZ = 0;
		int chunkSize = 0;
		int[,] numPointsAtFrame;
		List<List<uint[]>> cacheData;
		int[,] chunkDataPointers;
		List<MaterialPropertyBlock> materialProperties = new List<MaterialPropertyBlock>();
		List<ComputeBuffer> chunksBuffer = new List<ComputeBuffer>();
		Vector3[] chunksCenter;
		int frame = 0;
		float deltaTime = 0.0f;
		string renderPipe = "";

	    void Start(){
	    	this.reset();
	    	
	    	string renderPipeAsset = "";
			if(GraphicsSettings.renderPipelineAsset != null){
				renderPipeAsset = GraphicsSettings.renderPipelineAsset.GetType().Name;
			}
			
			if(renderPipeAsset == "HDRenderPipelineAsset"){
				this.renderPipe = "hdrp";
			}
			else if(renderPipeAsset == "UniversalRenderPipelineAsset"){
				this.renderPipe = "urp";
			}
			else{
				this.renderPipe = "builtin";
			}

			this.loadClaymationFile();
	    }

	     void OnDestroy(){
	     	this.reset();
	    }

	    public void loadClaymationFile(){
	    	this.reset();

	    	if(this.claymationFile == null){
	    		this.loaded = false;
	    		return;
	    	}

	    	MemoryStream stream = new MemoryStream(this.claymationFile.bytes);
	    	BinaryReader reader = new BinaryReader(stream);

	    	// if(Application.isPlaying){
	    	// 	this.claymationFile = null; // unload original file from memory?
	    	// }
	        
	        this.fileFormat = reader.ReadInt32();
	        this.chunkSize = reader.ReadInt32();
	        this.chunksX = reader.ReadInt32();
	        this.chunksY = reader.ReadInt32();
	        this.chunksZ = reader.ReadInt32();
	        this.numFrames = reader.ReadInt32();

	        this.numChunks = this.chunksX * this.chunksY * this.chunksZ;
	        
	        this.numPointsAtFrame = new int[this.numFrames, this.numChunks];
	        this.chunkDataPointers = new int[this.numFrames, this.numChunks];

	        int[] maxChunkPointCount = new int[this.numChunks];

	        this.cacheData = new List<List<uint[]>>(this.numFrames);
			
	        for(int frameIt = 0; frameIt < this.numFrames; ++frameIt){
	        	List<uint[]> chunkData = new List<uint[]>(this.numChunks);
	        	this.cacheData.Add(chunkData);

	        	for(int chunkIt_ = 0; chunkIt_ < this.numChunks; ++chunkIt_){
					int numPoints = reader.ReadInt32();
					
					this.numPointsAtFrame[frameIt, chunkIt_] = numPoints;

					if(numPoints > maxChunkPointCount[chunkIt_]){
						maxChunkPointCount[chunkIt_] = numPoints;
					}

					chunkData.Add(new uint[numPoints * 2]);

					for(int pointIt = 0; pointIt < numPoints; ++pointIt){
						chunkData[chunkIt_][pointIt * 2] = reader.ReadUInt32();
						chunkData[chunkIt_][(pointIt * 2) + 1] = reader.ReadUInt32();
					}
				}
	        }
			
	        reader.Close();

	        if(this.material == null){
	        	if(this.renderPipe == "hdrp"){
					this.material = new Material(Shader.Find("Clayxels/ClayxelHDRPShader"));
				}
				else if(this.renderPipe == "urp"){
					this.material = new Material(Shader.Find("Clayxels/ClayxelURPShader"));
				}
				else{
					this.material = new Material(Shader.Find("Clayxels/ClayxelBuiltInShader"));
				}

	        	Texture texture = this.material.GetTexture("_MainTex");
				if(texture == null){
					this.material.SetTexture("_MainTex", (Texture)Resources.Load("clayxelDot"));
				}
			}

	        this.material.SetFloat("chunkSize", (float)this.chunkSize);

	        this.boundsScale = new Vector3(
	        	(float)this.chunkSize * this.chunksX,
				(float)this.chunkSize * this.chunksY,
				(float)this.chunkSize * this.chunksZ);

			float seamOffset = this.chunkSize / 256.0f; 
			float chunkOffset = this.chunkSize - seamOffset;
			float gridCenterOffset = (this.chunkSize * 0.5f);

			this.chunksCenter = new Vector3[this.numChunks];

			this.materialProperties.Clear();
			this.chunksBuffer.Clear();

			int chunkIt = 0;
	        for(int z = 0; z < this.chunksZ; ++z){
				for(int y = 0; y < this.chunksY; ++y){
					for(int x = 0; x < this.chunksX; ++x){
			        	int chunkPoints = maxChunkPointCount[chunkIt];
			        	
			        	if(chunkPoints > 0){
			        		this.materialProperties.Add(new MaterialPropertyBlock());

			        		this.chunksBuffer.Add(new ComputeBuffer(chunkPoints, sizeof(int) * 2));

							this.chunksCenter[chunkIt] = new Vector3(
								(-((this.chunkSize * this.chunksX) * 0.5f) + gridCenterOffset) + (chunkOffset * x),
								(-((this.chunkSize * this.chunksY) * 0.5f) + gridCenterOffset) + (chunkOffset * y),
								(-((this.chunkSize * this.chunksZ) * 0.5f) + gridCenterOffset) + (chunkOffset * z));

							MaterialPropertyBlock materialPropBlock = this.materialProperties[chunkIt];
	    					materialPropBlock.SetBuffer("chunkPoints", this.chunksBuffer[chunkIt]);
							materialPropBlock.SetVector("chunkCenter",  this.chunksCenter[chunkIt]);
			        	}
			        	else{
			        		this.chunksBuffer.Add(null);
			        		this.materialProperties.Add(null);
			        	}

			        	chunkIt += 1;
			        }
			    }
	        }

	        this.voxelSize = ((float)this.chunkSize / 256);

	        this.loadFrame(0);

	        this.loaded = true;
	    }

	    public int getFrame(){
	    	return this.frame;
	    }

	    public int getNumFrames(){
	    	return this.numFrames;
	    }

	    public void loadFrame(int frame){
	    	this.frame = frame;

	    	if(this.frame < 0){
	    		this.frame = 0;
	    	}
	    	else if(this.frame > this.numFrames - 1){
	    		this.frame = this.numFrames - 1;
	    	}
			
	    	for(int chunkIt = 0; chunkIt < this.chunksBuffer.Count; ++chunkIt){
	    		ComputeBuffer chunkBuffer = this.chunksBuffer[chunkIt];
	    		if(chunkBuffer != null){
	    			chunkBuffer.SetData(this.cacheData[this.frame][chunkIt], 0, 0, this.numPointsAtFrame[this.frame, chunkIt] * 2);
	    		}
	    	}
	    }

	    #if UNITY_EDITOR
	    void Awake(){
	    	this.reset();

	    	if(!this.loaded){
	    		return;
	    	}

	    	if(!Application.isPlaying){
	    		if(this.chunksBuffer.Count == 0){
	    			this.reset();

	    			this.loadClaymationFile();
	    		}
	    	}
	    }
	    #endif

	    void reset(){
	    	for(int chunkIt = 0; chunkIt < this.chunksBuffer.Count; ++chunkIt){
	    		ComputeBuffer chunkBuffer = this.chunksBuffer[chunkIt];
	    		if(chunkBuffer != null){
	    			chunkBuffer.Release();
	    		}
	    	}

	    	this.chunksBuffer.Clear();
	    	this.numChunks = 0;
	    	this.loaded = false;
	    }

	    void Update(){
	    	if(!this.loaded){
	    		return;
	    	}

	    	#if UNITY_EDITOR
	    		// fix for unity clearing buffers on loose/regain focus
		    	if(!Application.isPlaying){
			    	if(this.numChunks > 0 && this.numPointsAtFrame == null){
			    		this.loadClaymationFile();
			    	}
			    }
		    #endif

			if(this.playAnim){
				if(this.numFrames > 1){
					this.deltaTime += Time.deltaTime;

					if(this.deltaTime > 1.0f / this.frameRate){
						this.frame = (this.frame + 1) % this.numFrames;
				    	this.loadFrame(this.frame);
				    	
						this.deltaTime = 0.0f;
					}
				}
			}

			this.renderBounds.center = this.transform.position;
	    	this.renderBounds.size = this.boundsScale * this.transform.lossyScale.x;

	    	this.material.SetMatrix("objectMatrix", this.transform.localToWorldMatrix);

	    	this.splatRadius = this.voxelSize * ((this.transform.lossyScale.x + this.transform.lossyScale.y + this.transform.lossyScale.z) / 3.0f);
			this.material.SetFloat("splatRadius", this.splatRadius);

	    	for(int chunkIt = 0; chunkIt < this.numChunks; ++chunkIt){
	    		int numPoints = this.numPointsAtFrame[this.frame, chunkIt];
	    		if(numPoints > 0){
		    		MaterialPropertyBlock materialProperties = this.materialProperties[chunkIt];
		    		
		    		#if UNITY_EDITOR // fix for disappearing clayxels in editor on certain events
		    			this.material.SetFloat("chunkSize", (float)this.chunkSize);
		    			materialProperties.SetBuffer("chunkPoints", this.chunksBuffer[chunkIt]);
						materialProperties.SetVector("chunkCenter",  this.chunksCenter[chunkIt]);
		    		#endif
	    			
					Graphics.DrawProcedural(
						this.material, 
						this.renderBounds,
						MeshTopology.Triangles, numPoints * 3, 0,
						null, materialProperties,
						ShadowCastingMode.On, true, this.gameObject.layer);
				}
	    	}
	    }
	}
}