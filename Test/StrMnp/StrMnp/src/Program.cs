using System;
using System.Collections.Generic;
using System.Linq;

namespace TestInterview
{

    public class Data
    {
        public char Character { get; set; }
        public int Counter { get; set; }
    }
    public class Child
    {
        public int ID { get; set; }
        public List<Data> Datas { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var tes = new List<string>()
            {
                "defghi",
                "idefgh",
                "idefgeh",
                "Abc*&3jfa",
                "123jdjynkhkrmdaaaa&*&9ui8888"
            };
            
            foreach(var i in tes)
            {
                Test(i);
            }
        }
        public static void Test(string x)
        {
            List<Data> datas = new List<Data>();
            x = x.ToLower();

            // Check ASCII
            foreach (var i in x)
            {
                if ((i > 47 && i < 58) || (i > 64 && i < 91) || (i > 96 && i < 123))
                {
                    Data n = new Data();
                    n.Character = (char)i;
                    int index = 0;
                    for (int j = 0; j < x.Length; j++)
                    {
                        if (i == x[j])
                        {
                            n.Counter = n.Counter + 1;
                        }

                    }
                    index++;
                    datas.Add(n);
                }

            }
            // Remove Duplicate
            datas = datas.OrderByDescending(o => o.Counter).GroupBy(o => o.Character).Select(o => o.First()).OrderBy(o => o.Counter).ToList();

            // Divide ?
            List<Child> children = new List<Child>();
            int ID = 0;
            foreach (var i in datas.GroupBy(o => o.Counter).Select(o => o.First()).ToList())
            {
                Child child = new Child();
                child.Datas = new List<Data>();
                child.ID = ID;
                foreach (var j in datas.Where(o => o.Counter == i.Counter).ToList())
                {

                    Data data = new Data();
                    data.Character = j.Character;
                    data.Counter = j.Counter;
                    child.Datas.Add(data);
                }
                ID++;
                children.Add(child);
            }

            // Print Result
            foreach (var i in children)
            {
                // ASC
                children[i.ID].Datas = children[i.ID].Datas.OrderBy(o => o.Character).ToList();
                foreach (var j in children[i.ID].Datas)
                {
                    Console.Write(j.Character);
                }
            }
            Console.WriteLine();
        }
    }
}
