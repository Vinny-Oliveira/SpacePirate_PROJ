
using UnityEngine;


public static class ClayxelsPrefs {
    // change the path to the retopology lib if Clayxels is not in the root of your Assets folder (winwdows only)
    public static string retopoLib = "Assets\\Clayxels\\Editor\\retopo\\retopoLib.dll";

    public static void apply(){
        Clayxels.ClayContainer.boundsColor = new Color(0.5f, 0.5f, 1.0f, 0.1f);
        Clayxels.ClayContainer.pickingKey = "p";

        // max number of points per chunk (number of chunks is set via boundsScale on the inspector)
        // this only affects video memory while sculpting or moving clayObjects at runtime
        // points are also automatically optimized whenever possible.
        Clayxels.ClayContainer.setPointCloudLimit(300000);

    	// max number of solids in a container
        // affects video memory
        Clayxels.ClayContainer.setMaxSolids(512);// 64, 128, 256, 512, 1024, 4096, 16384

        // max size of the container work area
        // only has effect in the UI, no performance change
        Clayxels.ClayContainer.setMaxChunks(4, 4, 4);
        
        // how many solids you can have in a single voxel
        // lower values means faster and less video memory used
        Clayxels.ClayContainer.setMaxSolidsPerVoxel(128);// 32, 64, 128, 256, 512, 1024, 2048

        // skip a certain amount of frames before updating
        // useful to make large containers go faster in game
        Clayxels.ClayContainer.setUpdateFrameSkip(0);
    }
}
