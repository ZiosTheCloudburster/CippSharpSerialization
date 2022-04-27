
using System;
using System.Collections.Generic;
using CippSharp.Core.Containers;
using CippSharp.Core.Extensions;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using CippSharp.Core.EditorUtils;
using UnityEditor;
#endif

namespace CippSharp.Serialization
{
	[CreateAssetMenu(menuName = nameof(CippSharp)+"/Data Assets/Binary Holder")]
    public class BinaryHolder : AHiddenListDataAsset<byte>
    {
        /// <summary>
        /// A nicer contextual name usable for logs.
        /// </summary>
        public static readonly string LogName = $"[{typeof(BinaryHolder).Name}]: ";
        
        /// <summary>
        /// Save the full type of the serialized object.
        /// </summary>
        [SerializeField] public string fullType = "";
        
        /// <summary>
        /// The stored bytes.
        /// </summary>
        //[SerializeField, HideInInspector] 
        private List<byte> bytes
        {
	        get => value;
	        set => this.value = value;
        }
	    
        /// <summary>
        /// Returns an array copy of the stored bytes;
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return bytes.ToArray();
        }
        
        /// <summary>
        /// Retrieve the length of the array of the stored bytes.
        /// </summary>
        public int Length
        {
            get
            {
                return bytes.Count;
            }
        }
        
        /// <summary>
        /// Tries to take bytes from any object. It must be a serializable type.
        /// </summary>
        /// <param name="target"></param>
        public bool Serialize(object target)
        {
            if (target == null) 
            {
                return false;
            }

            fullType = target.GetType().FullName;
	        var result = SerializationUtils.Serialize(target, out value, this);
#if UNITY_EDITOR
	        EditorUtility.SetDirty(this);
#endif
            return result;
        }

        /// <summary>
        /// Tries to convert stored bytes in T.
        /// </summary>
        /// <typeparam name="T">T full type should match the stored full type.
        /// and/or it should be assignable from T</typeparam>
        /// <returns></returns>
        public T Deserialize<T>()
        {
            if (string.IsNullOrEmpty(fullType))
            {
                Debug.LogError(LogName+"You're trying to deserialize but type is null or empty.", this);
                return default(T);
            }

            Type type = typeof(T);
            if (type.FullName != fullType)
            {
                Debug.LogError(LogName+"You're trying to deserialize but a type that doesn't match with the stored one.", this);
                return default(T);
            }

            T target;
            SerializationUtils.Deserialize(bytes.ToArray(), out target, this);
            return target;
        }

	    #region Custom Editor
#if UNITY_EDITOR
	    [CustomEditor(typeof(BinaryHolder))]
	    private class ObjectHolderEditor : Editor
	    {
		    private BinaryHolder binaryHolder = null;
		    private SerializedProperty fullType = null;

		    private int elementsPerPage = 32;
		    [SerializeField, HideInInspector] private int pageIndexBackingField = 0;

		    private int PageIndex
		    {
			    get { return pageIndexBackingField; }
			    set
			    {
				    if (pageIndexBackingField == value)
				    {
					    return;
				    }

				    pageIndexBackingField = value;
				    int index = pageIndexBackingField * elementsPerPage;
				    int dataLength = binaryHolder.Length;
				    if (index + elementsPerPage < dataLength)
				    {
					    inspectedElements = binaryHolder.GetBytes().SubArrayOrDefault(index, elementsPerPage);
				    }
				    else
				    {
					    inspectedElements = binaryHolder.GetBytes()
						    .SubArrayOrDefault(index, Mathf.Clamp(dataLength - index, 0, elementsPerPage));
				    }

				    try
				    {
					    if (!inspectedElements.IsNullOrEmpty())
					    {
						    encoded = System.Text.Encoding.UTF8.GetString(inspectedElements);
					    }
				    }
				    catch (Exception e)
				    {
					    Debug.LogError(e.Message);
				    }
			    }
		    }

		    [SerializeField, HideInInspector] private byte[] inspectedElements = new byte[0];
#pragma warning disable 414
		    [SerializeField, TextArea(1, 2), HideInInspector]
		    private string encoded = "";
#pragma warning restore 414
		    private SerializedObject serializedThis = null;
		    private SerializedProperty ser_inspectedElements = null;
		    private SerializedProperty ser_encoded = null;

		    private void OnEnable()
		    {
			    binaryHolder = (BinaryHolder) target;
			    fullType = serializedObject.FindProperty(nameof(BinaryHolder.fullType));

			    serializedThis = new SerializedObject(this);
			    ser_inspectedElements = serializedThis.FindProperty(nameof(inspectedElements));
			    ser_encoded = serializedThis.FindProperty(nameof(encoded));
		    }

		    public override void OnInspectorGUI()
		    {
			    EditorGUILayoutUtils.DrawScriptReferenceField(serializedObject);
			    serializedObject.Update();
			    serializedThis.Update();

			    EditorGUILayoutUtils.DrawNotEditableProperty(fullType);
			    int length = binaryHolder.Length;
			    if (length > 0)
			    {
				    ser_inspectedElements.isExpanded = EditorGUILayout.Foldout(ser_inspectedElements.isExpanded,
					    ser_inspectedElements.displayName, EditorStyles.foldout);
				    if (ser_inspectedElements.isExpanded)
				    {
					    int pagesLength = (Mathf.CeilToInt((float) length / (float) elementsPerPage)) - 1;
					    if (PageIndex > pagesLength)
					    {
						    Debug.LogWarning("Pages index out of range! Last page will be drawn instead.");
					    }

					    EditorGUI.indentLevel++;
					    PageIndex = Mathf.Clamp(PageIndex, 0, pagesLength);
					    PageIndex = EditorGUILayout.IntSlider(PageIndex, 0, pagesLength);

					    EditorGUILayout.LabelField($"Displaying page: {PageIndex.ToString()}/{pagesLength.ToString()}.");
					    EditorGUI.indentLevel++;
					    bool guiEnabled = GUI.enabled;
					    GUI.enabled = false;
					    for (int i = 0; i < ser_inspectedElements.arraySize; i++)
					    {
						    SerializedProperty element = ser_inspectedElements.GetArrayElementAtIndex(i);
						    EditorGUILayoutUtils.DrawProperty(element);
					    }

					    GUI.enabled = guiEnabled;
					    EditorGUILayout.LabelField($"Displaying elements: {inspectedElements.Length.ToString()}/{elementsPerPage.ToString()}");
					    EditorGUI.indentLevel--;
					    EditorGUILayoutUtils.DrawProperty(ser_encoded);
					    EditorGUI.indentLevel--;
				    }
			    }

			    EditorGUILayoutUtils.DrawHeader("Commands:");
			    EditorGUI.indentLevel++;
			    EditorGUILayoutUtils.DrawMiniButton("Save as Text", SaveAsTextFile);
			    
			    EditorGUILayoutUtils.DrawMiniButton("Set Dirty", () => {EditorUtility.SetDirty(target);});
			    EditorGUI.indentLevel--;
			    serializedThis.ApplyModifiedProperties();
			    serializedObject.ApplyModifiedProperties();
		    }
	
		    private void SaveAsTextFile()
		    {
			    try
			    {
				    string file = System.Text.Encoding.UTF8.GetString(binaryHolder.GetBytes());
				    string fullPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(binaryHolder)) + "/" +
				                      binaryHolder.name + "Text" + ".txt";
				    File.WriteAllText(fullPath, file);
			    }
			    catch (Exception e)
			    {
				    Debug.LogError(e.Message);
			    }
		    }
	    }
#endif
	    #endregion
    }
}
