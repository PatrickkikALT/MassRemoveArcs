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
    public class RemoveArcsPlugin
    {
        private Texture2D texture2D;
        private Sprite sprite;
        public static bool IsArc(BaseObject o) => o is BaseArc;
        private ExtensionButton button;
        [Init]
        private void Init()
        {
            Debug.Log("RemoveArcs has loaded");
            LoadSprite();
            button = new ExtensionButton
            {
                Icon = sprite,
                Tooltip = "Remove Selected Arcs"
            };
            button.Click += RemoveArcs;
            ExtensionButtons.AddButton(button);
        }
        public void LoadSprite()
        {
            if (texture2D == null)
            {
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MassRemoveArcs.Images.image.png"))
                {
                    var dataLen = (int)stream.Length;
                    var data = new byte[dataLen];
                    stream.Read(data, 0, dataLen);

                    texture2D = new Texture2D(512, 512);
                    texture2D.LoadImage(data);
                }
                if (sprite == null)
                    sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0, 0), 100.0f, 0, SpriteMeshType.Tight);
            }
        }
        public void RemoveArcs()
        {
            var foundArcs = SelectionController.SelectedObjects.Where(IsArc).Cast<BaseArc>().ToList();
            
            if (foundArcs.Count == 0) return;
            
            var collection = BeatmapObjectContainerCollection.GetCollectionForType(foundArcs[0].ObjectType);
            collection.RemoveConflictingObjects(foundArcs);
            foreach (var arc in foundArcs)
            {
                collection.DeleteObject(arc, triggersAction: false);
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
