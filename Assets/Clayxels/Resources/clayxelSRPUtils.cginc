
#ifdef SHADERPASS // detect the shadow pass in HDRP and URP
	#if SHADERPASS == SHADERPASS_SHADOWS
		#define SHADERPASS_SHADOWCASTER
	#endif
#endif

#ifdef UNITY_PASS_SHADOWCASTER // detect shadow pass in built-in
	#define SHADERPASS_SHADOWCASTER
#endif

#if defined (SHADER_API_D3D11) || defined(SHADER_API_METAL) || (SHADER_API_GLES) || (SHADER_API_GLES3) || (SHADER_API_PS4) || (SHADER_API_XBOXONE)
	#define CLAYXELS_VALID
#endif

#ifdef CLAYXELS_VALID
	uniform StructuredBuffer<int2> chunkPoints;
	uniform StructuredBuffer<int> pointCloudDataToSolidId;
#endif 

float4x4 objectMatrix;
float3 chunkCenter;
float chunkSize = 0.0;
float splatRadius = 0.01;
int solidHighlightId;
int chunkId;
int chunkMaxOutPoints;

static const float vOffsetUpTable[3] = {-1.0, -1.0, 1.7};
static const float vOffsetSideTable[3] = {1.0, -1.0, 0.0};
static const float4 vTexTable[3] = {
	 float4(-0.5, 0.0, 0.0, 0.0),
	 float4(1.5, 0.0, 0.0, 0.0),
	 float4(0.5, 1.35, 0.0, 0.0)
};

int bytes4ToInt(uint a, uint b, uint c, uint d){
	int retVal = (a << 24) | (b << 16) | (c << 8) | d;
	return retVal;
}

int4 unpackInt4(uint inVal){
	uint r = inVal >> 24;
	uint g = (0x00FF0000 & inVal) >> 16;
	uint b = (0x0000FF00 & inVal) >> 8;
	uint a = (0x000000FF & inVal);

	return int4(r, g, b, a);
}

float3 unpackFloat3(float f){
	return frac(f / float3(16777216, 65536, 256));
}

float4 unpackR6G6B6A14(uint value){
	float a = ((float(value & 0x3FFF) / 16383) * 2.0) - 1.0;
	value >>= 14;
	float b = float(value & 0x3f) / 63;
	value >>= 6;
    float g = float(value & 0x3f) / 63;
    float r = float(value >> 6) / 63;
   	
    return float4(r, g, b, a);
}


float3 expandGridPoint(int3 cellCoord, float cellSize, float localChunkSize){
	float cellCornerOffset = cellSize * 0.5;
	float halfBounds = localChunkSize * 0.5;
	float3 gridPoint = float3(
		(cellSize * cellCoord.x) - halfBounds, 
		(cellSize * cellCoord.y) - halfBounds, 
		(cellSize * cellCoord.z) - halfBounds) + cellCornerOffset;

	return gridPoint;
}

float2 unpackFloat2(float input){
	int precision = 32;
	float2 output = float2(0.0, 0.0);

	output.y = input % precision;
	output.x = floor(input / precision);

	return output / (precision - 1);
}

float3 unpackNormal(float fSingle){
	float2 f = unpackFloat2(fSingle);

	f = f * 2.0 - 1.0;

	float3 n = float3( f.x, f.y, 1.0 - abs( f.x ) - abs( f.y ) );
	float t = saturate( -n.z );
	n.xy += n.xy >= 0.0 ? -t : t;

	return normalize( n );
}

float3 unpackNormal2Byte(uint value1, uint value2){
	float2 f = float2((float)value1 / 256.0, (float)value2 / 256.0);

	f = f * 2.0 - 1.0;

	float3 n = float3( f.x, f.y, 1.0 - abs( f.x ) - abs( f.y ) );
	float t = saturate( -n.z );
	n.xy += n.xy >= 0.0 ? -t : t;

	return normalize( n );
}

float3 unpackRgb(uint inVal){
	int r = (inVal & 0x000000FF) >>  0;
	int g = (inVal & 0x0000FF00) >>  8;
	int b = (inVal & 0x00FF0000) >> 16;

	return float3(r/255.0, g/255.0, b/255.0);
}

void unpack66668(int value, out int4 unpackedData1, out int unpackedData2){
	unpackedData2 = value & 0x000000FF;
	value >>= 8;
	unpackedData1.w = value & 0x3f;
	value >>= 6;
	unpackedData1.z = value & 0x3f;
	value >>= 6;
	unpackedData1.y = value & 0x3f;
	value >>= 6;
	unpackedData1.x = value & 0x3f;
}

void clayxelGetPointCloud(uint vId, out float3 gridPoint, out float3 pointColor, out float3 pointCenter, out float3 pointNormal){
#ifdef CLAYXELS_VALID
	int pointId = vId / 3;
	int2 clayxelPointData = chunkPoints[pointId];
	
	int4 data1 = unpackInt4(clayxelPointData.x);
	int4 data2;
	int data3;
	unpack66668(clayxelPointData.y, data2, data3);

	float3 normal = unpackNormal2Byte(data1.w, data3);

	float cellSize = chunkSize / 256.0;
	float halfCell = cellSize * 0.5;

	float normalOffset = (((data2.x / 64.0) * 4.0) - 1.0) * halfCell;
	
	float3 cellOffset = float3(cellSize*0.5, cellSize*0.5, cellSize*0.5) + (normal * normalOffset);

	gridPoint = expandGridPoint(data1.xyz, cellSize, chunkSize);

	float3 pointPos = gridPoint + cellOffset + chunkCenter;
	pointCenter = mul(objectMatrix, float4(pointPos, 1.0)).xyz;
	
	pointNormal = mul((float3x3)objectMatrix, normal);
	
	pointColor = float3((float)data2.y / 64.0, (float)data2.z / 64.0, (float)data2.w / 64.0);
#else
	gridPoint = float3(0, 0, 0);
	pointColor = float3(0, 0, 0);
	pointCenter = float3(0, 0, 0);
	pointNormal = float3(0, 0, 0);
#endif
}

void clayxelVertNormalBlend(uint vId, float splatSizeMult, float normalOrientedSplat, out float4 tex, out float3 vertexColor, out float3 outVertPos, out float3 outNormal){
#ifdef CLAYXELS_VALID
	// first we unpack the clayxels point cloud
	int pointId = vId / 3;
	int2 clayxelPointData = chunkPoints[pointId];
	
	int4 data1 = unpackInt4(clayxelPointData.x);
	int4 data2;
	int data3;
	unpack66668(clayxelPointData.y, data2, data3);

	float3 normal = unpackNormal2Byte(data1.w, data3);

	float cellSize = chunkSize / 256.0;
	float halfCell = cellSize * 0.5;

	float normalOffset = (((data2.x / 64.0) * 4.0) - 1.0) * halfCell;
	
	float3 cellOffset = float3(cellSize*0.5, cellSize*0.5, cellSize*0.5) + (normal * normalOffset);
	float3 pointPos = expandGridPoint(data1.xyz, cellSize, chunkSize) + cellOffset + chunkCenter;
	float3 p = mul(objectMatrix, float4(pointPos, 1.0)).xyz;
	
	outNormal = mul((float3x3)objectMatrix, normal);
	
	vertexColor = float3((float)data2.y / 64.0, (float)data2.z / 64.0, (float)data2.w / 64.0);

	if(solidHighlightId > -1){
		int solidId = pointCloudDataToSolidId[(chunkId * chunkMaxOutPoints) + pointId];
		if(solidId == solidHighlightId + 1){
			vertexColor += 1.0;
		}
	}

	float newSplatSize = splatRadius * splatSizeMult * 0.9;
	float3 camUpVec = UNITY_MATRIX_V._m10_m11_m12;
	float3 camSideVec = UNITY_MATRIX_V._m00_m01_m02;
	
	float3 upVec;
	float3 sideVec;

	#if defined(SHADERPASS_SHADOWCASTER) // on shadowPass force splats orientating to normals to prevent holes in the shadows
		sideVec = normalize(cross(camUpVec, outNormal)) * (newSplatSize * 2.0);
		upVec = normalize(cross(sideVec, outNormal)) * newSplatSize;
	#else
		float3 normalSideVec = normalize(cross(camUpVec, outNormal));
		float3 normalUpVec = normalize(cross(normalSideVec, outNormal));
		
		upVec = normalize(lerp(camUpVec, normalUpVec, normalOrientedSplat)) * (newSplatSize);
		sideVec = normalize(lerp(camSideVec, normalSideVec, normalOrientedSplat)) * (newSplatSize * 2.0);
	#endif

	// expand splat from point P to a triangle with uv coordinates
	uint vertexOffset = vId % 3;
	outVertPos = p + ((upVec * vOffsetUpTable[vertexOffset]) + (sideVec * vOffsetSideTable[vertexOffset]));
	tex = vTexTable[vertexOffset];

	#if !defined(SHADERPASS_SHADOWCASTER) 
		float3 eyeVec = normalize(_WorldSpaceCameraPos - p);
		outVertPos = outVertPos - (eyeVec * (dot(eyeVec, outVertPos - p)));
	#endif
#else
	tex = float4(0, 0, 0, 0);
	vertexColor = float3(0, 0, 0);
	outVertPos = float3(0, 0, 0);
	outNormal = float3(0, 0, 0);
#endif
}

// keeping this for compatibility with old code, this function is no longer
void clayxelVertFoliage(uint vId, float splatSizeMult, float normalOrientedSplat, out float4 tex, out float3 vertexColor, out float3 outVertPos, out float3 outNormal){
	clayxelVertNormalBlend(vId, splatSizeMult, normalOrientedSplat, tex, vertexColor, outVertPos, outNormal);
}

float random(float2 uv){
    return frac(sin(dot(uv,float2(12.9898,78.233)))*43758.5453123);
}

void srpSplatTexture(float2 vertexUV, float textureAlpha, out float outAlpha){
	outAlpha = textureAlpha;

	if(textureAlpha > 0.0){
		if(random(vertexUV) > textureAlpha){
			outAlpha = 0.0;
		}
		else{
			outAlpha = 1.0;
		}
	}
}

void clayxelFrag(float3 vertexColor, float4 vertexUV, float textureAlpha, out float outAlpha){
	outAlpha = 1.0;

	#if SPLATTEXTURE_ON // if textured splats, randomly discard pixels based on alpha amount
		srpSplatTexture(vertexUV.xy, textureAlpha, outAlpha);
	#else// default is round point splat, just discard around radius
		if(length(vertexUV.xy -0.5) > 0.5){
			outAlpha = 0.0;
		}
	#endif
}
