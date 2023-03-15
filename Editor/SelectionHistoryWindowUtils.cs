using UnityEditor;

namespace SelectionHistoryWindow
{
	public class SelectionHistoryWindowUtils {

		public static readonly string HistorySizePrefKey = "Coquid.SelectionHistory.HistorySize";
		public static readonly string HistoryAutomaticRemoveDeletedPrefKey = "Coquid.SelectionHistory.AutomaticRemoveDeleted";
		public static readonly string HistoryAllowDuplicatedEntriesPrefKey = "Coquid.SelectionHistory.AllowDuplicatedEntries";
	    public static readonly string HistoryShowHierarchyObjectsPrefKey = "Coquid.SelectionHistory.ShowHierarchyObjects";
	    public static readonly string HistoryShowProjectViewObjectsPrefKey = "Coquid.SelectionHistory.ShowProjectViewObjects";

	    public static readonly string HistoryShowPinButtonPrefKey = "Coquid.SelectionHistory.ShowFavoritesPinButton";

	    public static readonly string ShowUnloadedObjectsKey = "Coquid.SelectionHistory.ShowUnloadedObjects";
	    public static readonly string ShowDestroyedObjectsKey = "Coquid.SelectionHistory.ShowDestroyedObjects";

	    private static readonly bool debugEnabled = false;

	    private bool showProjectViewObjects;
		
		public static bool AutomaticRemoveDeleted =>
			EditorPrefs.GetBool(HistoryAutomaticRemoveDeletedPrefKey, true);
		
		public static bool AllowDuplicatedEntries =>
			EditorPrefs.GetBool(HistoryAllowDuplicatedEntriesPrefKey, false);

		public static bool ShowHierarchyViewObjects =>
			EditorPrefs.GetBool(HistoryShowHierarchyObjectsPrefKey, true);
		
		public static bool ShowUnloadedObjects =>
			EditorPrefs.GetBool(ShowUnloadedObjectsKey, true);
		
		public static bool ShowDestroyedObjects =>
			EditorPrefs.GetBool(ShowDestroyedObjectsKey, false);
		
		public static bool ShowFavoriteButton =>
			EditorPrefs.GetBool(HistoryShowPinButtonPrefKey, false);
	

	    public static void PingEntry(SelectionHistory.Entry e)
	    {
		    if (e.GetReferenceState() == SelectionHistory.Entry.State.ReferenceUnloaded)
		    {
			    var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(e.scenePath);
			    EditorGUIUtility.PingObject(sceneAsset);
		    } else
		    {
			    EditorGUIUtility.PingObject(e.reference);
		    }
	    }
	}
}