using System.Collections.Generic;

namespace BluApi.Common
{
    /// <summary>
    /// Function class is a tricky name, there is no "function" in C#, they are called "method" and defined by return type ;)
    /// This class just structures a multiple return type when a methods returns more than one type
    /// We use alias "Return Type" (rt) for method internal
    /// Using "Function" as a name makes method naming a bit familiar looking for non C# programmers
    /// This project is also aimed for Chef programmers who are not neccesarily a big fan of C# namings
    /// e.g. 
    /// public Function myCat()
    ///    {
    ///         ReturnType rt = new ReturnType();
    ///         if (myCatEat())
    ///         {
    ///             rt.Result = 0;
    ///             rt.Object = myCat;
    ///             rt.Dictionary = animals;
    ///             rt.Message = "good, my cat eats.";
    ///         }
    ///         else
    ///         {
    ///             rt.Result = 0;
    ///             rt.Object = myCat;
    ///             rt.Dictionary = animals;
    ///             rt.Message = "Omg, my cat stopped eating :(";
    ///         }
    ///         return rt;
    ///    }
    /// </summary>
    public class Function
    {
        public int Result { get; set; }
        public object Object { get; set; }
        public string Data { get; set; }
        public string Message { get; set; }
        public Dictionary<string, dynamic> Dictionary { get; set; }
    }
}
