using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace SelectionHistoryWindow
{
    public class FavoriteAssetsWindow : EditorWindow
    {
        [MenuItem("Tools/SelectionHistory/Favorites")]
        public static void OpenWindow()
        {
            var window = GetWindow<FavoriteAssetsWindow>();
            var titleContent = EditorGUIUtility.IconContent(UnityBuiltInIcons.favoriteIconName);
            titleContent.text = "Favorites";
            titleContent.tooltip = "Favorite assets window";
            window.titleContent = titleContent;
        }

        [MenuItem("Assets/Favorite Item")]
        [Shortcut("Coquid/Favorite Item", null, KeyCode.F, ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
        public static void Favorite()
        { 
            FavoriteElements(Selection.objects);
        }

        private static void FavoriteElements(Object[] references)
        {
            var favorites = FavoritesController.Favorites;

            foreach (var reference in references)
            {
                if (favorites.IsFavorite(reference))
                    continue;
            
                if (favorites.CanBeFavorite(Selection.activeObject))
                {
                    favorites.AddFavorite(new Favorites.Favorite
                    {
                        reference = reference
                    });   
                }
            }
        }

        private Favorites _favorites;

        public StyleSheet styleSheet;

        public VisualTreeAsset favoriteElementTreeAsset;
        
        private void OnDisable()
        {
            if (_favorites != null)
            {
                _favorites.OnFavoritesUpdated -= OnFavoritesUpdated;
            }
        }

        public void OnEnable()
        {
            _favorites = FavoritesController.Favorites;
            _favorites.OnFavoritesUpdated += OnFavoritesUpdated;
            
            var root = rootVisualElement;
            root.styleSheets.Add(styleSheet);

            root.RegisterCallback<DragPerformEvent>(evt =>
            {
                DragAndDrop.AcceptDrag();
                FavoriteElements(DragAndDrop.objectReferences);
            });
            
            root.RegisterCallback<DragUpdatedEvent>(evt =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            });
            
            ReloadRoot();
        }

        private void OnFavoritesUpdated(Favorites favorites)
        {
            var root = rootVisualElement;
            root.Clear();
        
            ReloadRoot();
        }

        private void ReloadRoot()
        {
            var root = rootVisualElement;

            // var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/FavoriteElement.uxml");

            var scroll = new ScrollView(ScrollViewMode.Vertical);
            root.Add(scroll);

            for (var i = 0; i < _favorites.favoritesList.Count; i++)
            {
                var assetReference = _favorites.favoritesList[i].reference;

                if (assetReference == null)
                    continue;
                
                var elementTree = favoriteElementTreeAsset.CloneTree();

                var dragArea = elementTree.Q<VisualElement>("DragArea");
                if (dragArea != null)
                {
#if !UNITY_EDITOR_OSX
                        dragArea.RegisterCallback<MouseUpEvent>(evt =>
                        {
                            if (evt.button == 0)
                            {
                                Selection.activeObject = assetReference;
                            }
                            else
                            {
                                EditorGUIUtility.PingObject(assetReference);
                            }
                            
                            dragArea.userData = null;
                        });
                        dragArea.RegisterCallback<MouseDownEvent>(evt =>
                        {
                            DragAndDrop.PrepareStartDrag();
                            DragAndDrop.objectReferences = new Object[] { null };
                            
                            dragArea.userData = true;
                        });
                        dragArea.RegisterCallback<MouseLeaveEvent>(evt =>
                        {
                            var dragging = false;
                            
                            if (dragArea.userData != null)
                            {
                                dragging = (bool) dragArea.userData;
                            }
                            
                            if (dragging)
                            {
                                DragAndDrop.PrepareStartDrag();
                                DragAndDrop.StartDrag("Dragging");
                                DragAndDrop.objectReferences = new Object[] {assetReference};
                            }
                        });
                        
                        dragArea.RegisterCallback<DragUpdatedEvent>(evt =>
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                        });
#else
                        dragArea.RegisterCallback<MouseUpEvent>(evt =>
                        {
                            if (evt.button == 0)
                            {
                                Selection.activeObject = assetReference;
                            }
                            else
                            {
                                EditorGUIUtility.PingObject(assetReference);
                            }
                        });
#endif
                }
                
                var icon = elementTree.Q<Image>("Icon");
                if (icon != null)
                {
                    icon.image = AssetPreview.GetMiniThumbnail(assetReference);
                }
                
                var removeIcon = elementTree.Q<Image>("RemoveIcon");
                if (removeIcon != null)
                {
                    // removeIcon.image = AssetPreview.GetMiniThumbnail(assetReference);
                    removeIcon.image = EditorGUIUtility.IconContent(UnityBuiltInIcons.removeIconName).image;
                    
                    removeIcon.RegisterCallback(delegate(MouseUpEvent e)
                    {
                        FavoritesController.Favorites.RemoveFavorite(assetReference);
                    });
                }
                
                var label = elementTree.Q<Label>("Favorite");
                if (label != null)
                {
                    label.text = assetReference.name;
                }

                scroll.Add(elementTree);
            }

            var receiveDragArea = new VisualElement();
            receiveDragArea.style.flexGrow = 1;
            root.Add(receiveDragArea);
        }
    }
}