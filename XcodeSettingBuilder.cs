using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using System.Collections.Generic;

// Xcodeの設定を自動で行うクラス
public static class XcodeSettingBuilder
{
	[PostProcessBuild(100)]
	public static void OnPostProcessBuild(BuildTarget target, string path)
	{
		if (target == BuildTarget.iOS)
		{
			PostProcessBuild_iOS (path);
		}
	}

	private static void PostProcessBuild_iOS (string path)
	{
		string projectPath = PBXProject.GetPBXProjectPath(path);
		PBXProject pbxProject = new PBXProject();
		
		pbxProject.ReadFromString(File.ReadAllText(projectPath));
		string target = pbxProject.TargetGuidByName("Unity-iPhone");

		// Frameworkの設定
		string [] frameworks = {"AVFoundation.framework",
								"AVKit.framework",
								"CoreLocation.framework",
								"CoreMedia.framework",
								"CoreTelephony.framework",
								"AdSupport.framework",
								"AVFoundation.framework",
								"MediaPlayer.framework"};
		pbxProject.AddFrameworkToProject(target, framewrok, false);

		// BuildSettingの設定
		pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
		pbxProject.SetBuildProperty(target, "SWIFT_VERSION", "Swift 4.0");

		// ARCの設定
		List<string> compile_Flags = new List<string>();
		compile_Flags.Add("-fno-objc-arc");
		pbxProject.SetCompileFlagsForFile(target, pbxProject.FindFileGuidByProjectPath("Libraries/Plugins/iOS/UNDialogManager.mm"), compile_Flags);
		pbxProject.SetCompileFlagsForFile(target, pbxProject.FindFileGuidByProjectPath("Libraries/Plugins/iOS/ADGNI.mm"), compile_Flags);

		// Pathの設定
		pbxProject.SetBuildProperty(target, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");
		pbxProject.SetBuildProperty(target, "SWIFT_OBJC_BRIDGING_HEADER", "Libraries/Plugins/iOS/Unity-iPhone-Bridging-Header.h");

		File.WriteAllText(projectPath, pbxProject.WriteToString());
	}
}