using UnityEngine;
using UnityEditor;
using FSM;
using System.Reflection;
using System.Linq;

public class FSMNodeEditor : EditorWindow
{

    StateMachine machine;
    Color backgroundColor = new Color(.25f, .255f, .26f);
    Color inspectorColor = new Color(.8f, .8f, .8f, .8f);
    Color nodeColor = new Color(.8f, .8f, .8f);
    Color entryNodeColor = Color.HSVToRGB(.6f, .5f, 1);
    bool IsMakingTransition = false;
    int activeNode = 0;
    int parametersWidth = 250;
    int selectedTransition = 0;
    bool deleteNode = false;
    string[] actionTypesNames;
    System.Type[] actionTypes;



    [MenuItem("Window/FSM editor")]
    public static void ShowEditor()
    {
        FSMNodeEditor editor = EditorWindow.GetWindow<FSMNodeEditor>();        
        editor.Init();
    }

    public void Init()
    {
        Object[] selection = Selection.GetFiltered(typeof(StateMachine), SelectionMode.Assets);
        if (selection.Length > 0)
        {
            machine = (StateMachine)selection[0];
        }

        var subclassTypes = Assembly.GetAssembly(typeof(Action)).GetTypes().Where(t => t.IsSubclassOf(typeof(Action)));
        
        actionTypesNames = new string[subclassTypes.Count()];
        actionTypes = new System.Type[subclassTypes.Count()];
        int i = 0;
        foreach (var subclass in subclassTypes)
        {
            actionTypesNames[i] = subclass.ToString();
            actionTypes[i] = subclass;
            i++;
        }
    }

    void OnGUI()
    {

        GUI.color = backgroundColor;
        GUI.DrawTexture(new Rect(0, 0, position.width, position.height), EditorGUIUtility.whiteTexture);

        Event e = Event.current;
        Vector2 mousePos = e.mousePosition;
        if (IsMakingTransition)
        {
            if (e.button == 0 && e.type == EventType.MouseDown)
            {
                for (int i = 0; i < machine.states.Count; i++)
                {
                    State state = machine.states[i];
                    if (state.rect.Contains(mousePos))
                    {
                        machine.states[activeNode].AddTransition(i);
                        break;
                    }
                }
                IsMakingTransition = false;
            }
        }
        else
        {
            if (e.button == 1 && e.type == EventType.MouseDown)
            {
                RightClick(e);
            }
            if (e.button == 0 && e.type == EventType.MouseDown)
            {
                LeftClick(e);
            }
        }
        
        GUI.color = nodeColor;
        BeginWindows();
        for (int i = 0; i < machine.states.Count; i++)
        {
            if (i == activeNode && deleteNode)
                continue;
            State state = machine.states[i];
            GUI.color = nodeColor;
            if (i == machine.entryState)
                GUI.color = entryNodeColor;
            state.rect = GUI.Window(i + 1, state.rect, DrawNodeWindow, state.name, GUI.skin.window);
            foreach (Transition trans in state.transitions)
            {
                DrawNodeCurve(state.rect, machine.states[trans.destination].rect);
            }
        }
        GUI.color = inspectorColor;
        Rect parametersRect = new Rect(position.width - parametersWidth, 0, parametersWidth, position.height);
        GUI.Window(0, parametersRect, DrawNodeWindow, "Parameters");
        GUI.BringWindowToFront(0);
        EndWindows();

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Delete)
        {
            deleteNode = true;
            Repaint();
        }

        if (deleteNode && e.type == EventType.Repaint)
        {
            machine.DeleteState(activeNode);
            activeNode = 0;
            deleteNode = false;
        }

    }
    
    private void RightClick(Event e)
    {
        GenericMenu menu;
        Vector2 mousePos = e.mousePosition;
        for (int i = 0; i < machine.states.Count; i++)
        {
            State state = machine.states[i];
            if (state.rect.Contains(mousePos))
            {
                menu = new GenericMenu();
                menu.AddItem(new GUIContent("Add Transition"), false, AddTransition, i);
                menu.ShowAsContext();
                return;
            }
        }
        menu = new GenericMenu();
        menu.AddItem(new GUIContent("Add State"), false, AddNode, mousePos);
        menu.ShowAsContext();
    }

    private void LeftClick(Event e)
    {
        Vector2 mousePos = e.mousePosition;
        for (int i = 0; i < machine.states.Count; i++)
        {
            State state = machine.states[i];
            if (state.rect.Contains(mousePos))
            {
                activeNode = i;
                selectedTransition = 0;
                return;
            }
        }
    }

    private void AddNode(object pos)
    {
        Vector2 mousePos = (Vector2)pos;
        machine.AddState(mousePos);
        Repaint();
    }

    private void AddTransition(object index)
    {
        activeNode = (int)index;
        IsMakingTransition = true;
        Debug.Log("transition");
    }

    void DrawNodeWindow(int id)
    {
        if (id > 0)
        {
            machine.states[id - 1].name = EditorGUILayout.TextField(machine.states[id - 1].name);
            if (GUILayout.Button("Entry State"))
            {
                machine.entryState = id - 1;
            }
            GUI.DragWindow();
        }
        else
        {
            DrawInspector();
        }
        //new Rect(0, 0, 10000, 20)
    }

    private void DrawInspector()
    {
        if (machine.states == null || machine.states.Count == 0)
            return;
        State state = machine.states[activeNode];
        GUILayout.Label(state.name, EditorStyles.boldLabel);

        string[] transitionTexts = new string[state.transitions.Count];
        for (int i = 0; i < state.transitions.Count; i++)
            transitionTexts[i] = machine.states[state.transitions[i].destination].name;
        
        selectedTransition = GUILayout.SelectionGrid(selectedTransition, transitionTexts, 1);
        if(state.transitions.Count > 0)
        {
            EditorGUI.BeginChangeCheck();
            string trigger = EditorGUILayout.TextField("Trigger", state.transitions[selectedTransition].trigger);
            if (EditorGUI.EndChangeCheck())
            {
                Transition trans = state.transitions[selectedTransition];
                trans.trigger = trigger;
                state.transitions[selectedTransition] = trans;
            }
            
        }
        EditorGUI.BeginChangeCheck();
        int actionIndex = EditorGUILayout.Popup(state.actionIndex, actionTypesNames);
        if (EditorGUI.EndChangeCheck())
        {
            state.actionIndex = actionIndex;
            Action action = (Action)System.Activator.CreateInstance(actionTypes[actionIndex]);
            //state.action = action;
            state.actionType = actionTypes[actionIndex].ToString();
            Debug.Log(action);
            
            //Debug.Log(System.Type.GetType(ActionTypes[actionIndex]));
            //state.action = GetInstance<Action>(ActionTypes[actionIndex]);
        }

    }


    void DrawNodeCurve(Rect start, Rect end)
    {
        Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.white, null, 3);
    }
}