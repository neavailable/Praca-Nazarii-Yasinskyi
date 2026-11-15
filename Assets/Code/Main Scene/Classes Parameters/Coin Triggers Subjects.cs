using System.Collections.Generic;
using R3;


namespace MainScene.ClassesParameters
{
    public class CoinTriggersSubjects
    {
        public Stack<Subject<int>> CoinsTriggersSubjects { get; private set; }


        public CoinTriggersSubjects(Stack<Subject<int>> coinsTriggersSubjects)
        {
            CoinsTriggersSubjects = coinsTriggersSubjects;
        }
    }
}