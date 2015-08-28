using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVManager2.gg
{

    public class LinkNode<T>
    {
        public LinkNode(T value)
        {
            this.Value = value;
        }

        public T Value { get; set; }

        public LinkNode<T> Next { get; set; }

        public LinkNode<T> Prev { get; set; }

        [FiledName("aaaaaaaaaa")]
        public string MyProperty { get; set; }

        //public void Test(Func<T, T, int> action, T a, T b)
        //{
        //    //action.Invoke(a, b);
        //    action += (T a1, T b1) =>
        //    {
        //        Console.WriteLine("aaaaaaaaa = {0}, bbbbbbbb = {1}", a1, b1);
        //        return 1;
        //    };
        //    action(a, b);
        //}

        //public int Test1(T a, T b)
        //{
        //    Console.WriteLine("a = {0}, b = {1}", a, b);
        //    return 0;
        //}
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class FiledNameAttribute : Attribute
    {

        public FiledNameAttribute(string a)
        {
            
        }
    }

    public class LinkNodeList : IEnumerable
    {

        public LinkNode<string> First { get; set; }

        public LinkNode<string> Last { get; set; }

        public void AddListNode(string value)
        {
            LinkNode<string> node = new LinkNode<string>(value);
            if(this.First == null)
            {
                this.First = node;
                node.Prev = Last;
                Last = First;
            }
            else
            {
                LinkNode<string> prev = Last;
                Last.Next = node;
                Last = node;
                Last.Prev = prev;
            }

            //node.Test(node.Test1, "aa", "bb");
        }

        public IEnumerator GetEnumerator()
        {
            LinkNode<string> current = this.First;
            while (current != null)
            {
                yield return current.Value;
                current = current.Next;
            }
        }
    }
}
