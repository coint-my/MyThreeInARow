using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPairTypeCount
{
    public int myCount;

    public MyTypeGem myType;

    public MyPairTypeCount() { }

    public MyPairTypeCount(MyTypeGem _type, int _count) 
    { 
        myType = _type;
        myCount = _count;
    }
}

public class my_source_gem : MonoBehaviour
{
    public bool MyIsHaveGem { get { return transform.childCount > 0; } }

    private my_gem myPrefabGem;

    private my_active_cell_move myCellMove;

    private List<MyPairTypeCount> listTypeCount;

    private void Start()
    {
        myPrefabGem = Resources.Load<my_gem>("my_prefab/my_gem");

        listTypeCount = new List<MyPairTypeCount>();
        listTypeCount.Add(new MyPairTypeCount(MyTypeGem.ROMB, 0));
        listTypeCount.Add(new MyPairTypeCount(MyTypeGem.RECT, 0));
        listTypeCount.Add(new MyPairTypeCount(MyTypeGem.OVAL, 0));
        listTypeCount.Add(new MyPairTypeCount(MyTypeGem.CIRCLE, 0));
        listTypeCount.Add(new MyPairTypeCount(MyTypeGem.OCTAGON, 0));
        //listTypeCount.Add(new MyPairTypeCount(MyTypeGem.TRIANGLE, 0));

        myCellMove = GetComponent<my_active_cell_move>();

        if (!MyIsHaveGem)
            MyAddGemRandom();
    }

    public void MyAddGemRandom()
    {
        //print("sourceGem MyAddGemRandom");
        if (!MyIsHaveGem)
        {
            my_gem gem = Instantiate(myPrefabGem);
            gem.MySpawnGem((MyTypeGem)UnityEngine.Random.Range(0, listTypeCount.Count));
            myCellMove.MyAddedGem(gem);
        }
        //print("end sourceGem MyAddGemRandom");
    }

    public void MyAddGemRandom(MyTypeGem _type)
    {
        if (!MyIsHaveGem)
        {
            my_gem gem = Instantiate(myPrefabGem);
            gem.MySpawnGem(_type);
            myCellMove.MyAddedGem(gem);
        }
    }

    public void MyAddGemCalculated(ref List<my_active_cell_move> _listCell)
    {
        //print("MyAddGemCalculated");
        try
        {
            if (_listCell.Count > 0 && !MyIsHaveGem)
            {
                for (int index = 0; index < listTypeCount.Count; index++)
                    listTypeCount[index].myCount = 0;

                for (int index = 0; index < _listCell.Count; index++)
                {
                    if (_listCell[index].myGem)
                    {
                        for (int pairIndex = 0; pairIndex < listTypeCount.Count; pairIndex++)
                        {
                            if (listTypeCount[pairIndex].myType == _listCell[index].myGem.MyGetType)
                            {
                                listTypeCount[pairIndex].myCount = listTypeCount[pairIndex].myCount + 1;
                                break;
                            }
                        }
                    }
                }

                listTypeCount.Sort(new MySortListGemCount());

                MyAddGemRandom(listTypeCount[UnityEngine.Random.Range(0, 2)].myType);
            }
        }
        catch(System.Exception _ex)
        {
            print("Exeption = " + _ex.Message);
        }
        //print("end MyAddGemCalculated");
    }
}
