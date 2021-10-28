using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView _graphView;
    private string _fileName = "New Narrative";
    private MiniMap _miniMap;
    private string path = "Assets/Resources/GraphData/previousGraph.txt";

    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void OnEnable()
    {
        string previousGraph = LoadPreviousGraph();
        if (previousGraph.Equals(""))
        {
            MakeNewGraph();
        }
        else
        {
            _fileName = previousGraph;
            MakeNewGraph();
            RequestDataOperation(false);
        }
    }

    private void MakeNewGraph()
    {
        ConstructGraphView();
        GenerateToolBar();
        GenerateMiniMap();
        GenerateBlackboard();
    }

    private void GenerateBlackboard()
    {
        var _blackboard = new Blackboard(_graphView);
        _blackboard.Add(new BlackboardSection { title = "Exposed Properties" });
        
        _blackboard.editTextRequested = (blackboard1, element, newPropertyName) =>
        {
            var oldPropertyName = ((BlackboardField)element).text;
            if (_graphView.ExposedProperties.Any(x => x.PropertyName.ToLower().Equals(newPropertyName.ToLower())))
            {
                EditorUtility.DisplayDialog("Error", "This property name already exists, please choose another one!",
                    "OK");
                return;
            }

            var propertyIndex = _graphView.ExposedProperties.FindIndex(x => x.PropertyName.Equals(oldPropertyName));
            var property = _graphView.ExposedProperties[propertyIndex];
            property.PropertyName = newPropertyName;
            ((BlackboardField)element).text = newPropertyName;
            if (property.PropertyType.Equals("Boolean"))
            {
                _graphView.oldConditionName = oldPropertyName;
                _graphView.newConditionName = newPropertyName;                
                _graphView.RefreshDropdown(property.PropertyType);
            }
        };
        
        _blackboard.SetPosition(new Rect(10, 30, 200, 300));
        _blackboard.scrollable = true;
        _graphView.Add(_blackboard);
        _graphView.Blackboard = _blackboard;
        _graphView.AddPropertySearchWindow(_graphView.EditorWindow);
    }

    private void GenerateMiniMap()
    {
        _miniMap = new MiniMap{ anchored = true };
        _miniMap.SetPosition(new Rect(_graphView.EditorWindow.position.width - 210, 30, 200, 140));
        _graphView.Add(_miniMap);
    }
    
    private void Update()
    {
        _miniMap.SetPosition(new Rect(_graphView.EditorWindow.position.width - 210, 30, 200, 140));
    }

    private void OnDisable()
    {
        RequestDataOperation(true);
        SavePreviousGraph(_fileName);
        rootVisualElement.Clear();
    }

    private void ConstructGraphView()
    {
        _graphView = new DialogueGraphView(this)
        {
            name = "Dialogue Graph"
        };

        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void GenerateToolBar()
    {
        var toolbar = new Toolbar();
        var fileNameTextField = new TextField("File Name:");
        fileNameTextField.SetValueWithoutNotify(_fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
        toolbar.Add(fileNameTextField);

        toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save Data" });
        toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load Data" });
        toolbar.Add(new Button(() => _graphView.OpenNodeSearchWindow()) { text = "New Node" });
        toolbar.Add(new Button(() => NewGraphButton()) { text = "New Graph" });

        rootVisualElement.Add(toolbar);
    }

    private void NewGraphButton()
    {
        OnDisable();
        _fileName = "New Narrative";
        MakeNewGraph();
    }

    public void RequestDataOperation(bool save)
    {
        if (string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name", "Please enter a valid file name.", "OK");
            return;
        }

        var saveUtility = GraphSaveUtility.GetInstance(_graphView);

        if (save)
        {
            saveUtility.SaveGraph(_fileName);
        }
        else
        {
            saveUtility.LoadGraph(_fileName);
        }
    }

    private void SavePreviousGraph(string previousGraph)
    {
        File.Create(path).Close();
        StreamWriter writer = new StreamWriter(path);
        writer.WriteLine(previousGraph);
        writer.Close();
    }
    
    private string LoadPreviousGraph()
    {
        if (!File.Exists(path))
        {
            FileInfo file = new FileInfo(path);
            file.Directory.Create();
            SavePreviousGraph("");
        }
        
        StreamReader reader = new StreamReader(path); 
        string fileContent = reader.ReadLine();
        reader.Close();
        return fileContent;
    }
}