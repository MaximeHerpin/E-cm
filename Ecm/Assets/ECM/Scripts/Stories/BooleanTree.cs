using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stories
{
    public abstract class BooleanNode
    {
        public BooleanNode left;
        public BooleanNode right;
        
        public BooleanNode(BooleanNode left=null, BooleanNode right = null)
        {
            this.left = left;
            this.right = right;
        }

        public abstract bool Eval();

        public abstract void Print();
    }

    public class AndNode : BooleanNode
    {
        public AndNode(BooleanNode left, BooleanNode right) : base(left, right) { }

        public override bool Eval()
        {
            if (left == null || right == null)
            {
                Debug.LogError("Boolean expression is not correct and can't be evaluated");
            }
            return left.Eval() && right.Eval();
        }

        public override void Print()
        {
            if (left != null)
                left.Print();
            Debug.Log(" AND ");
            if (right != null)
                right.Print();
        }
    }

    public class OrNode : BooleanNode
    {
        public OrNode(BooleanNode left, BooleanNode right) : base(left, right) { }

        public override bool Eval()
        {
            if (left == null || right == null)
            {
                Debug.LogError("Boolean expression is not correct and can't be evaluated");
            }
            return left.Eval() || right.Eval();
        }

        public override void Print()
        {
            if (left != null)
                left.Print();
            Debug.Log(" OR ");
            if (right != null)
                right.Print();
        }

    }

    public class NotNode : BooleanNode
    {
        public NotNode() { }

        public override bool Eval()
        {
            if (left == null || right != null)
            {
                Debug.LogError("Boolean expression is not correct and can't be evaluated");
            }
            return ! left.Eval();
        }

        public override void Print()
        {
            Debug.Log(" NOT ");
            if (left != null)
                left.Print();
        }

    }

    public class LeafNode : BooleanNode
    {
        private string variableName;

        public LeafNode(string variableName)
        {
            this.variableName = variableName;
        }

        public override bool Eval()
        {
            return Conditions.instance.conditions[variableName];
        }

        public override void Print()
        {
            Debug.Log(variableName);
        }

    }

    public class BooleanExpression
    {
        private BooleanNode root;
        public string expression;

        public BooleanExpression(string expression)
        {
            this.expression = expression;
            StringToBooleanTree();
        }

        public bool Eval()
        {
            if (root == null)
                return true;
            return root.Eval();
        }

        public void StringToBooleanTree()
        {
            string[] words = expression.Split(' ');
            if (words.Length == 0 || expression.Length == 0)
                return;

            Queue<string> output = new Queue<string>();
            Stack<string> operators = new Stack<string>();

            foreach(string word in words)
            {
                switch (word)
                {
                    case "OR":                     
                        while(operators.Count > 0 && operators.Peek() != "OR" && operators.Peek() != "(")
                        {
                            output.Enqueue(operators.Pop());
                        }
                        operators.Push(word);
                        break;
                    case "(":
                    case "AND":
                        operators.Push(word);
                        break;
                    case ")":
                        while (operators.Count > 0 && operators.Peek() != "(")
                        {
                            output.Enqueue(operators.Pop());
                        }
                        operators.Pop();
                        break;
                    default:
                        output.Enqueue(word);
                        break;
                }
            }
            while(operators.Count > 0)
            {
                output.Enqueue(operators.Pop());
            }

            Stack<BooleanNode> nodes = new Stack<BooleanNode>();
            foreach (string word in output)
            {
                BooleanNode node;
                if (word == "OR" || word == "AND")
                {
                    BooleanNode right = nodes.Pop();
                    BooleanNode left = nodes.Pop();

                    if (word == "OR")
                        node = new OrNode(left, right);
                    else node = new AndNode(left, right);
                }
                else
                {
                    node = new LeafNode(word);
                    Conditions.instance.conditions[word] = false;
                }
                nodes.Push(node);
            }
            root = nodes.Pop();
        }
    }



}
