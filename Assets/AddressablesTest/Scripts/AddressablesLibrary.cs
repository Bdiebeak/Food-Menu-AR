using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AddressablesTest.Scripts
{
    [Serializable]
    public class AssetReferenceTextAsset : AssetReferenceT<TextAsset>
    {
        public AssetReferenceTextAsset(string guid) : base(guid) { }
    }

    [Serializable]
    public class AddressableNode
    {
        public string name;
        public AssetReferenceTextAsset file;
    }

    [CreateAssetMenu(fileName = nameof(AddressablesLibrary), menuName = "ScriptableObjects/" + nameof(AddressablesLibrary))]
    public class AddressablesLibrary : ScriptableObject
    {
        [SerializeField]
        private List<AddressableNode> assets = new();

        public int GetAssetsLength()
        {
            return assets.Capacity;
        }

        public AddressableNode GetAsset(int index)
        {
            return assets[index];
        }
    }
}
