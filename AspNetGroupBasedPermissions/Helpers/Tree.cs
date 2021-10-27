using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetGroupBasedPermissions.Helpers
{

    public class Tree<T>
    {
        private T data;
        private List<Tree<T>> children;

        public Tree(T data)
        {
            this.data = data;
            children = new List<Tree<T>>();
        }

        public void AddChild(T data)
        {
            children.Add(new Tree<T>(data));
        }

        public Tree<T> GetChild(int i)
        {
            foreach (Tree<T> n in children)
                if (--i == 0)
                    return n;
            return null;
        }

        public void Traverse(Tree<T> node, Action<T> visitor)
        {
            visitor(node.data);
            foreach (Tree<T> kid in node.children)
                Traverse(kid, visitor);
        }
    }
}