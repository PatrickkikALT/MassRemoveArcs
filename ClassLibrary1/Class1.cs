using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Beatmap.Base;
using Beatmap.Enums;
using Beatmap.V3;
using UnityEngine;

namespace RemoveArcs
{
    [Plugin("RemoveArcs")]
    public class Plugin
    {
        private Texture2D texture2D;
        private Sprite sprite;
        private static HashSet<BaseObject> SelectedObjects => SelectionController.SelectedObjects;
        public static bool IsArc(BaseObject o) => o is BaseArc;
        private ExtensionButton button = new ExtensionButton();
        [Init]
        private void Init()
        {
            Debug.Log("RemoveArcs has loaded");
            LoadSprite();
            button.Icon = sprite;
            button.Tooltip = "Remove Selected Arcs";
            button.Click += RemoveArcs;
            ExtensionButtons.AddButton(button);
        }
        internal bool affectsSeveralObjects = false;
        public void LoadSprite()
        {
            if (texture2D == null)
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MassRemoveArcs.Images.perfect.png"))
                {
                    int data_len = (int)stream.Length;
                    byte[] data = new byte[data_len];
                    stream.Read(data, 0, data_len);

                    texture2D = new Texture2D(512, 512);
                    texture2D.LoadImage(data);
                }
                if (sprite == null) sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0, 0), 100.0f, 0, SpriteMeshType.Tight);
            }
        }
        public void RemoveArcs()
        {
            var foundArcs = SelectedObjects.Where(IsArc).Cast<BaseArc>().ToList();
            foreach (var arc in foundArcs)
            {
                var collection = BeatmapObjectContainerCollection.GetCollectionForType(arc.ObjectType);
                collection.RemoveConflictingObjects(new[] { arc });
                collection.DeleteObject(arc, true, true, inCollectionOfDeletes: affectsSeveralObjects);
                
            }
            BeatmapActionContainer.AddAction(new SelectionDeletedAction(foundArcs));
        }
        [Exit]
        private void Exit()
        {
            Debug.Log("Program has closed.");
        }
    }
}
