using UnityEditor;
using UnityEngine;
using System.IO;
 
public static class ReverseAnimationContext
{
 
    [MenuItem("Assets/Create Reversed Clip", false, 14)]
    private static void ReverseClip()
    {
        string directoryPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject));
        string fileName = Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject));
        string fileExtension = Path.GetExtension(AssetDatabase.GetAssetPath(Selection.activeObject));
        fileName = fileName.Split('.')[0];
        string copiedFilePath = directoryPath + Path.DirectorySeparatorChar + fileName + "_Reversed" + fileExtension;
        var clip = GetSelectedClip();
 
        AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(Selection.activeObject), copiedFilePath);
 
        clip  = (AnimationClip)AssetDatabase.LoadAssetAtPath(copiedFilePath, typeof(AnimationClip));
 
        if (clip == null)
            return;
        float clipLength = clip.length;
        
        EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(clip);
        AnimationClipCurveData[] allCurves = new AnimationClipCurveData[curveBindings.Length];
        
        for (int index = 0; index < allCurves.Length; ++index)
        {
            allCurves[index] = new AnimationClipCurveData(curveBindings[index])
            {
                curve = AnimationUtility.GetEditorCurve(clip, curveBindings[index])
            };
        }
        
        clip.ClearCurves();
        
        foreach (AnimationClipCurveData curve in allCurves)
        {
            var keys = curve.curve.keys;
            int keyCount = keys.Length;
            (curve.curve.postWrapMode, curve.curve.preWrapMode) = (curve.curve.preWrapMode, curve.curve.postWrapMode);
            for (int i = 0; i < keyCount; i++)
            {
                Keyframe K = keys[i];
                K.time = clipLength - K.time;
                var tmp = -K.inTangent;
                K.inTangent = -K.outTangent;
                K.outTangent = tmp;
                keys[i] = K;
            }
            curve.curve.keys = keys;
            clip.SetCurve(curve.path, curve.type, curve.propertyName, curve.curve);
        }
        var events = AnimationUtility.GetAnimationEvents(clip);
        if (events.Length > 0)
        {
            foreach (var t in events)
            {
                t.time = clipLength - t.time;
            }

            AnimationUtility.SetAnimationEvents(clip, events);
        }
        Debug.Log("Animation reversed!");
    }
 
    [MenuItem("Assets/Create Reversed Clip", true)]
    static bool ReverseClipValidation()
    {
        return Selection.activeObject is AnimationClip;
    }
 
    public static AnimationClip GetSelectedClip()
    {
        var clips = Selection.GetFiltered(typeof(AnimationClip), SelectionMode.Assets);
        if (clips.Length > 0)
        {
            return clips[0] as AnimationClip;
        }
        return null;
    }
 
}
 