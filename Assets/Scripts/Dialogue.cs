using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine;

namespace DialogueSystem
{
    /// <summary>
    /// Represents a dialogue tree
    /// </summary>
    public sealed class Dialogue
    {
        Node[] m_Nodes;
        int m_Index;

        Node m_CurrentNode;

        /// <summary>
        /// Creates a dialogue from a set of strings.
        /// </summary>
        public Dialogue(List<string> lines)
        {
            var builder = new DialogueBuilder();

            if (!DialogueParser.ParseLines(builder, lines))
            {
                return;
            }

            m_Index = 0;

            var nodes = builder.GetNodesCopy();

            m_Nodes = nodes;
            m_CurrentNode = nodes[m_Index];
        }

        /// <summary>
        /// Reads dialogue from a set of files.
        /// </summary>
        /// <param name="filePaths"></param>
        /// <returns></returns>
        public static Dialogue ReadFromFile(params string[] filePaths)
        {
            var lines = new List<string>();

            foreach (var filePath in filePaths)
            {
                lines.AddRange(File.ReadAllLines(filePath));
            }

            return new Dialogue(lines);
        }

        /// <summary>
        /// Progresses the dialogue to the next node and returns it.
        /// </summary>
        public bool Next(out INode nextNode)
        {
            bool isAtEnd = GetNextNode(out var node);
            nextNode = node;
            return isAtEnd;
        }

        bool GetNextNode(out Node node)
        {
            node = null;

            m_Index = m_CurrentNode.GetNextIndex();

            if (m_Index > m_Nodes.Length - 1)
            {
                return false;
            }

            node = m_Nodes[m_Index];

            m_CurrentNode = node;

            return node is not EndNode;
        }

        sealed class DialogueBuilder
        {
            List<Node> m_Nodes = new List<Node>();

            int m_Index = 0;

            public Node[] GetNodesCopy()
            {
                return m_Nodes.ToArray();
            }

            public void Patch()
            {
                PatchJumpNodes(m_Nodes);
            }

            void EmitNode(Node node)
            {
                m_Nodes.Add(node);
                m_Index++;
            }

            public void EmitLabelNode(string label)
            {
                EmitNode(new LabelNode(m_Index, label));
            }

            public void EmitTextNode(string line)
            {
                EmitNode(new TextNode(m_Index, line));
            }

            public void EmitAskNode(string[] labels)
            {
                EmitNode(new AskNode(m_Index, labels));
            }

            public void EmitJumpByNode(int offset)
            {
                EmitNode(new JumpByNode(m_Index, offset));
            }

            public void EmitJumpToNode(int offset, string label)
            {
                EmitNode(new JumpToNode(m_Index, offset, label));
            }

            public void EmitDynamicEventNode(string name, DynamicEventArgs eventArgs)
            {
                EmitNode(new DynamicEventNode(m_Index, name, eventArgs));
            }

            public void EmitStartNode()
            {
                EmitNode(new StartNode(m_Index));
            }

            public void EmitEndNode()
            {
                EmitNode(new EndNode(m_Index));
            }

            public void EmitErrorNode(string error)
            {
                EmitNode(new ErrorNode(m_Index, error));
            }

            void PatchJumpNodes(List<Node> nodes)
            {
                var labels = new List<LabelNode>(nodes.OfType<LabelNode>());
                var jumpTos = new List<JumpToNode>(nodes.OfType<JumpToNode>());

                var labelToLabelNode = new Dictionary<string, LabelNode>();

                var patchedLabels = new List<LabelNode>();

                foreach (var label in labels)
                {
                    labelToLabelNode.Add(label.Label, label);
                }

                foreach (var jumpTo in jumpTos)
                {
                    if (labelToLabelNode.TryGetValue(jumpTo.Label, out var label))
                    {
                        patchedLabels.Add(label);

                        int offset = label.Index - jumpTo.Index;
                        jumpTo.Offset = offset;
                    }
                    else
                    {
                        Debug.LogWarning($"Could not find label definition for label reference \"{jumpTo.Label}\"");
                    }
                }

                var unpatchedLabels = labels.Except(patchedLabels);

                foreach(var label in unpatchedLabels)
                {
                    Debug.LogWarning($"Label \"{label.Label}\" is unreachable");
                }
            }
        }

        static class DialogueParser
        {
            readonly static Dictionary<string, Action<DialogueBuilder, string[], string, int>> m_ParseRules = new Dictionary<string, Action<DialogueBuilder, string[], string, int>>()
            {
                { "_ask", ParseAsk },
                { "_label", ParseLabel },
                { "_jump by", ParseJumpBy },
                { "_jump to", ParseJumpTo },
                { "_event", ParseDynamicEvent },
                { "_start", ParseStartNode },
                { "_end", ParseEndNode }
            };

            public static bool ParseLines(DialogueBuilder builder, List<string> lines)
            {
                builder.EmitEndNode();
                builder.EmitStartNode();

                for(int lineNumber = 0; lineNumber < lines.Count; lineNumber++)
                {
                    string line = lines[lineNumber].Trim();

                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    if (line[0] == '_')
                    {
                        string[] tokens = line.Split(';');

                        string node = tokens[0];
                        string[] args = tokens[1..];

                        if (m_ParseRules.TryGetValue(node.Trim(), out var parseRules))
                        {
                            parseRules(builder, args, line, lineNumber + 1);
                        }
                        else
                        {
                            Debug.LogWarning($"Node \"{node}\" is not valid");
                        }
                    }
                    else
                    {
                        builder.EmitTextNode(line);
                    }
                }

                builder.Patch();

                return true;
            }

            static bool ArgumentIsInvalid(string[] args, int index)
            {
                return args == null || index < 0 || index > args.Length - 1;
            }

            static bool TryParseInt(string[] args, int index, out int result)
            {
                result = 0;

                if (ArgumentIsInvalid(args, index))
                {
                    return false;
                }

                if (int.TryParse(args[index].Trim(), out result))
                {
                    return true;
                }
                else
                {
                    Debug.LogWarning($"Failed while parsing int. Expected whole number, got \"{args[index]}\"");
                    return false;
                }
            }

            static bool TryParseInt(string s, out int result)
            {
                result = 0;

                if (string.IsNullOrEmpty(s))
                {
                    return false;
                }

                if (int.TryParse(s.Trim(), out result))
                {
                    return true;
                }
                else
                {
                    Debug.LogWarning($"Failed while parsing int. Expected whole number, got \"{s}\"");
                    return false;
                }
            }

            static bool TryParseFloat(string[] args, int index, out float result)
            {
                result = 0;

                if (ArgumentIsInvalid(args, index))
                {
                    return false;
                }

                if (float.TryParse(args[index].Trim(), out result))
                {
                    return true;
                }
                else
                {
                    Debug.LogWarning($"Failed while parsing float. Expected int, got \"{args[index]}\"");
                    return false;
                }
            }

            static bool TryParseFloat(string s, out float result)
            {
                result = 0;

                if (string.IsNullOrEmpty(s))
                {
                    return false;
                }

                if (float.TryParse(s.Trim(), out result))
                {
                    return true;
                }
                else
                {
                    Debug.LogWarning($"Failed while parsing float. Expected float, got \"{s}\"");
                    return false;
                }
            }

            static bool TryParseString(string[] args, int index, out string result)
            {
                result = string.Empty;

                if (ArgumentIsInvalid(args, index))
                {
                    return false;
                }
                else
                {
                    result = args[index].Trim();
                    return true;
                }
            }

            static bool TryParseKeyValuePair(string[] args, int index, out string key, out string value, string line, int lineNumber)
            {
                key = string.Empty;
                value = string.Empty;

                if (ArgumentIsInvalid(args, index))
                {
                    FormatError("label reference", line, lineNumber, "Invalid arguments.");
                    return false;
                }

                string[] keyAndValue = args[index].Split('|');
                if (keyAndValue.Length != 2)
                {
                    FormatError("label reference", line, lineNumber, $"Expected 2 arguments got {keyAndValue.Length}.");
                    return false;
                }

                key = keyAndValue[0].Trim();
                value = keyAndValue[1].Trim();

                return true;
            }

            static void ParseAsk(DialogueBuilder dialogueBuilder, string[] args, string line, int lineNumber)
            {
                int labelReferenceCount = args.Length;

                string[] labelReferenceArray = new string[labelReferenceCount];
                string[] labelTextArray = new string[labelReferenceCount];

                for (int i = 0; i < labelReferenceCount; i++)
                {
                    if (!TryParseKeyValuePair(args, i, out labelReferenceArray[i], out labelTextArray[i], line, lineNumber))
                    {
                        dialogueBuilder.EmitErrorNode(FormatError("ask", line, lineNumber, "Invalid arguments."));
                        return;
                    }
                }

                dialogueBuilder.EmitAskNode(labelTextArray);

                for (int i = 0; i < labelReferenceCount; i++)
                {
                    dialogueBuilder.EmitJumpToNode(0, labelReferenceArray[i]);
                }
            }

            static void ParseLabel(DialogueBuilder dialogueBuilder, string[] args, string line, int lineNumber)
            {
                if (!TryParseString(args, 0, out var alias))
                {
                    dialogueBuilder.EmitErrorNode(FormatError("label", line, lineNumber, "Invalid arguments."));             
                    return;
                }

                dialogueBuilder.EmitLabelNode(alias);
            }

            static void ParseJumpBy(DialogueBuilder dialogueBuilder, string[] args, string line, int lineNumber)
            {
                if (!TryParseInt(args, 0, out var offset))
                {
                    dialogueBuilder.EmitErrorNode(FormatError("jump by", line, lineNumber, "Invalid arguments."));                 
                    return;
                }

                dialogueBuilder.EmitJumpByNode(offset);
            }

            static void ParseJumpTo(DialogueBuilder dialogueBuilder, string[] args, string line, int lineNumber)
            {
                if (!TryParseString(args, 0, out var label))
                {
                    dialogueBuilder.EmitErrorNode(FormatError("jump to", line, lineNumber, "Invalid arguments."));
                    return;
                }

                dialogueBuilder.EmitJumpToNode(0, label);
            }

            static void ParseDynamicEvent(DialogueBuilder dialogueBuilder, string[] args, string line, int lineNumber)
            {
                if (!TryParseString(args, 0, out var name))
                {
                    dialogueBuilder.EmitErrorNode(FormatError("dynamic event", line, lineNumber, "Invalid arguments."));   
                    return;
                }

                var eventArgsAsString = args[1..];

                var eventArgs = new object[eventArgsAsString.Length];

                for(int i = 0; i < eventArgsAsString.Length; i++)
                {
                    if (!TryParseKeyValuePair(eventArgsAsString, i, out var eventArgType, out var eventArgValueAsString, line, lineNumber))
                    {
                        dialogueBuilder.EmitErrorNode(FormatError("dynamic event", line, lineNumber, "Invalid arguments."));
                        return;
                    }

                    switch(eventArgType)
                    {
                        case "int":
                            {
                                if (TryParseInt(eventArgValueAsString, out var value))
                                {
                                    eventArgs[i] = value;
                                }
                            }
                            break;
                        case "float":
                            {
                                if (TryParseFloat(eventArgValueAsString, out var value))
                                {
                                    eventArgs[i] = value;
                                }
                            }
                            break;
                        case "string":
                            eventArgs[i] = eventArgValueAsString;
                            break;
                        default:
                            Debug.LogWarning($"Unknown type \"{eventArgType}\"");
                            break;
                    }
                }

                dialogueBuilder.EmitDynamicEventNode(name, new DynamicEventArgs(eventArgs));
            }

            static void ParseEndNode(DialogueBuilder dialogueBuilder, string[] args, string line, int lineNumber)
            {
                dialogueBuilder.EmitEndNode();
            }

            static void ParseStartNode(DialogueBuilder dialogueBuilder, string[] args, string line, int lineNumber)
            {
                dialogueBuilder.EmitEndNode();
            }

            static string FormatError(string node, string line, int lineNumber, string error)
            {
                string formattedError = $"Failed while parsing {node} \"{line}\" on line {lineNumber}. {error}";
                Debug.LogWarning(formattedError);
                return formattedError;
            }
        }

        /// <summary>
        /// The base class for all nodes.
        /// </summary>
        abstract class Node : INode
        {
            int INode.Index => Index;
            public readonly int Index;

            public Node(int index)
            {
                Index = index;
            }

            public virtual int GetNextIndex()
            {
                return Index + 1;
            }
        }

        /// <summary>
        /// The base class for jump nodes.
        /// </summary>
        abstract class JumpNode : Node, IJump
        {
            int IJump.Offset => Offset;
            public int Offset;

            protected JumpNode(int index, int offset) : base(index)
            {
                Offset = offset;
            }

            public override int GetNextIndex()
            {
                return Index + 1 + Offset;
            }
        }

        /// <summary>
        /// Marks a set location in the text.
        /// </summary>
        sealed class LabelNode : Node, ILabel
        {
            string ILabel.Label => Label;
            public readonly string Label;

            public LabelNode(int index, string label) : base(index)
            {
                Label = label;
            }

            public override string ToString()
            {
                return $"Label: Index {Index}, Offset 1, Text \"{Label}\"";
            }
        }

        /// <summary>
        /// Contains one line of text.
        /// </summary>
        sealed class TextNode : Node, IText
        {
            string IText.Text => Text;
            public readonly string Text;

            public TextNode(int index, string text) : base(index)
            {
                Text = text;
            }

            public override string ToString()
            {
                return $"Text: Index {Index}, Offset 1, Text \"{Text}\"";
            }
        }

        /// <summary>
        /// Asks a question and indexes a jump-table based on the answer to enable branching.
        /// </summary>
        sealed class AskNode : Node, IAsk
        {
            string[] IAsk.Answers => Answers;
            public readonly string[] Answers;
            int IAsk.AnswerIndex
            {
                get => GetAnswerIndex();
                set => SetAnswerIndex(value);
            }

            int m_AnswerIndex;

            public AskNode(int index, params string[] answers) : base(index)
            {
                Answers = answers;
            }

            public void SetAnswerIndex(int offset)
            {
                m_AnswerIndex = offset;
            }

            public int GetAnswerIndex()
            {
                return m_AnswerIndex;
            }

            public override int GetNextIndex()
            {
                return Index + 1 + m_AnswerIndex;
            }

            public override string ToString()
            {
                return $"Ask: Index {Index}, Offset {m_AnswerIndex} Answer Count {Answers.Length}";
            }
        }

        /// <summary>
        /// Jump by a certain distance.
        /// </summary>
        sealed class JumpByNode : JumpNode, IJumpBy
        {
            public JumpByNode(int index, int offset) : base(index, offset)
            {
            }

            public override string ToString()
            {
                return $"Jump To: Index {Index}, Offset {Offset}, Jumped to index {Index + Offset}";
            }
        }

        /// <summary>
        /// Jumps to a certain label.
        /// </summary>
        sealed class JumpToNode : JumpNode, IJumpTo
        {
            string IJumpTo.Label => Label;
            public readonly string Label;

            public JumpToNode(int index, int offset, string label) : base(index, offset)
            {
                Label = label;
            }

            public override string ToString()
            {
                return $"Jump To: Index {Index}, Offset {Offset}, Label {Label}, Jumped to index {Index + Offset}";
            }
        }

        /// <summary>
        /// A dynamic event invoked from a specific position of the text.
        /// </summary>
        sealed class DynamicEventNode : Node, IDynamicEvent
        {
            string IDynamicEvent.Name => Name;
            public readonly string Name;

            DynamicEventArgs IDynamicEvent.Args => Args;
            public readonly DynamicEventArgs Args;

            public DynamicEventNode(int index, string name, DynamicEventArgs args) : base(index)
            {
                Name = name;
                Args = args;
            }

            public override string ToString()
            {
                return $"Dynamic Event: Index {Index}, Name {Name}";
            }
        }

        /// <summary>
        /// Marks the start of a branch.
        /// </summary>
        sealed class StartNode : Node, IStart
        {
            public StartNode(int index) : base(index)
            {
            }

            public override string ToString()
            {
                return $"Start: Index {Index}";
            }
        }

        /// <summary>
        /// Marks the end of a branch.
        /// </summary>
        sealed class EndNode : Node, IEnd
        {
            public EndNode(int index) : base(index)
            {
            }

            public override string ToString()
            {
                return $"End: Index {Index}";
            }
        }

        /// <summary>
        /// Is emitted when the parser cant parse a node, contains the error message.
        /// </summary>
        sealed class ErrorNode : Node, IError
        {
            string IError.Error => Error;
            public readonly string Error;

            public ErrorNode(int index, string error) : base(index)
            {
                Error = error;
            }

            public override string ToString()
            {
                return $"Null: Index {Index}, Error {Error}";
            }
        }
    }

    /// <summary>
    /// Events corresponding to each node dialogue node.
    /// </summary>
    public sealed class DialogueEvent<T>
        where T : INode
    {
        event Action<Dialogue, T> m_Event;

        public void AddListener(Action<Dialogue, T> listener)
        {
            m_Event += listener;
        }

        public void RemoveListener(Action<Dialogue, T> listener)
        {
            m_Event -= listener;
        }

        public void Dispatch(Dialogue dialogue, T node)
        {
            m_Event?.Invoke(dialogue, node);
        }
    }

    /// <summary>
    /// Dispatches dialogue events.
    /// </summary>
    public sealed class DialogueEventDispatcher
    {
        public readonly DialogueEvent<ILabel> LabelCallback = new DialogueEvent<ILabel>();
        public readonly DialogueEvent<IText> TextCallback = new DialogueEvent<IText>();
        public readonly DialogueEvent<IAsk> AskCallback = new DialogueEvent<IAsk>();
        public readonly DialogueEvent<IJumpBy> JumpByCallback = new DialogueEvent<IJumpBy>();
        public readonly DialogueEvent<IJumpTo> JumpToCallback = new DialogueEvent<IJumpTo>();
        public readonly DialogueEvent<IDynamicEvent> DynamicEventCallback = new DialogueEvent<IDynamicEvent>();
        public readonly DialogueEvent<IStart> StartCallback = new DialogueEvent<IStart>();
        public readonly DialogueEvent<IEnd> EndCallback = new DialogueEvent<IEnd>();
        public readonly DialogueEvent<IError> ErrorCallback = new DialogueEvent<IError>();

        Dictionary<string, List<Action<DynamicEventArgs>>> m_DynamicEventListeners = new Dictionary<string, List<Action<DynamicEventArgs>>>();

        public void AddDynamicEventListener(string dynamicEvent, Action<DynamicEventArgs> listener)
        {
            if (m_DynamicEventListeners.TryGetValue(dynamicEvent, out var listeners))
            {
                listeners.Add(listener);
            }
            else
            {
                m_DynamicEventListeners.Add(dynamicEvent, new List<Action<DynamicEventArgs>>() { listener });
            }
        }

        public void RemoveDynamicEventListener(string dynamicEvent, Action<DynamicEventArgs> listener)
        {
            if (m_DynamicEventListeners.TryGetValue(dynamicEvent, out var listeners))
            {
                listeners.Remove(listener);
            }
        }

        void DispatchDynamicEvent(string dynamicEvent, DynamicEventArgs args)
        {
            if (m_DynamicEventListeners.TryGetValue(dynamicEvent, out var listeners))
            {
                foreach (var listener in listeners)
                {
                    listener(args);
                }
            }
        }

        public void TranslateAndDispatch(Dialogue dialogue, INode node)
        {
            switch (node)
            {
                case ILabel label:
                    LabelCallback.Dispatch(dialogue, label);
                    break;
                case IText text:
                    TextCallback.Dispatch(dialogue, text);
                    break;
                case IAsk ask:
                    AskCallback.Dispatch(dialogue, ask);
                    break;
                case IJumpBy jump:
                    JumpByCallback.Dispatch(dialogue, jump);
                    break;
                case IJumpTo jumpTo:
                    JumpToCallback.Dispatch(dialogue, jumpTo);
                    break;
                case IDynamicEvent dynamicEvent:
                    DynamicEventCallback.Dispatch(dialogue, dynamicEvent);
                    DispatchDynamicEvent(dynamicEvent.Name, dynamicEvent.Args);
                    break;
                case IStart startNode:
                    StartCallback.Dispatch(dialogue, startNode);
                    break;
                case IEnd endNode:
                    EndCallback.Dispatch(dialogue, endNode);
                    break;
                case IError error:
                    ErrorCallback.Dispatch(dialogue, error);
                    break;
            }
        }
    }

    /// <summary>
    /// Contains dynamic event args and functions to unpack them.
    /// </summary>
    public sealed class DynamicEventArgs
    {
        readonly object[] m_ArgArray;
        readonly DynamicEventArgType[] m_ArgTypeArray;

        public readonly int ArgCount;

        public DynamicEventArgs(object[] args)
        {
            ArgCount = args.Length;

            m_ArgArray = args;
            m_ArgTypeArray = new DynamicEventArgType[ArgCount];

            for (int i = 0; i < ArgCount; i++)
            {
                switch (args[i])
                {
                    case int:
                        m_ArgTypeArray[i] = DynamicEventArgType.Int;
                        break;
                    case float:
                        m_ArgTypeArray[i] = DynamicEventArgType.Float;
                        break;
                    case string:
                        m_ArgTypeArray[i] = DynamicEventArgType.String;
                        break;
                }
            }
        }

        public bool TryGet(int index, out int arg)
        {
            return TryGet(index, DynamicEventArgType.Int, out arg);
        }

        public bool TryGet(int index, out float arg)
        {
            return TryGet(index, DynamicEventArgType.Float, out arg);
        }

        public bool TryGet(int index, out string arg)
        {
            return TryGet(index, DynamicEventArgType.String, out arg);
        }

        bool TryGet<T>(int index, DynamicEventArgType expectedType, out T arg)
        {
            arg = default;

            if (index > ArgCount - 1)
            {
                return false;
            }

            if (m_ArgTypeArray[index] == expectedType)
            {
                arg = (T)m_ArgArray[index];
                return true;
            }
            else
            {
                Debug.LogWarning($"Error while unpacking argument {index}: Expected arg of type {expectedType}, got {m_ArgTypeArray[index]}.");
                return false;
            }
        }

        enum DynamicEventArgType
        {
            None,
            Int,
            Float,
            String
        }
    }

    public interface ILabel : INode
    {
        string Label { get; }
    }

    public interface INode
    {
        int Index { get; }
    }

    public interface IJump : INode
    {
        int Offset { get; }
    }

    public interface IText : INode
    {
        string Text { get; }
    }

    public interface IAsk : INode
    {
        string[] Answers { get; }
        int AnswerIndex { get; set; }
    }

    public interface IJumpBy : IJump
    {
    }

    public interface IJumpTo : IJump
    {
        string Label { get; }
    }

    public interface IDynamicEvent : INode
    {
        string Name { get; }
        DynamicEventArgs Args { get; }
    }

    public interface IStart : INode
    {
    }

    public interface IEnd : INode
    {
    }

    public interface IError : INode
    {
        string Error { get; }
    }
}
