using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MyDirection { UNKNOWN = -1, LEFT, RIGHT, UP, DOWN }

public struct MyPairInt
{
    public int x, y;
}

//public class MyTwoDArrayField
//{
//    private my_cell_script[,] myArrCell;

//    public my_cell_script[,] MyArrCell { get { return myArrCell; } }

//    public MyTwoDArrayField(my_cell_script[] _arrCell, int _wid, int _hei)
//    {
//        myArrCell = new my_cell_script[_wid, _hei];

//        MySortArray mySort = new MySortArray();
//        Array.Sort(_arrCell, mySort);

//        for (int y = 0; y < myArrCell.GetLength(1); y++)
//        {
//            for (int x = 0; x < myArrCell.GetLength(0); x++)
//            {
//                myArrCell[x, y] = _arrCell[(_hei * y) + x];
//            }
//        }
//    }

//    public void MyInitialize()
//    {
//        for (int y = 0; y < myArrCell.GetLength(1); y++)
//            for (int x = 0; x < myArrCell.GetLength(0); x++)
//                myArrCell[x, y].MyInitialize();
//    }

//    private void MyAddLineGemsCombo(my_active_cell_move _first, my_active_cell_move _second,
//        ref List<my_active_cell_move> _comboList, ref List<my_active_cell_move> _lineList)
//    {
//        if (_first != null && _second != null)
//        {
//            if (_first.MyIsHaveGem && _second.MyIsHaveGem)
//            {
//                //Debug.Log("combo  gem = " + _first.myGem);
//                if (_lineList.Count == 0 && (_first.myGem.MyGetType == _second.myGem.MyGetType))
//                {
//                    _lineList.Add(_first);
//                    _lineList.Add(_second);
//                }
//                else if (_lineList.Count > 0 && (_first.myGem.MyGetType == _second.myGem.MyGetType))
//                {
//                    _lineList.Add(_second);
//                }
//                else if (_lineList.Count > 2 && (_first.myGem.MyGetType != _second.myGem.MyGetType))
//                {
//                    _comboList.AddRange(_lineList);
//                    _lineList.Clear();
//                }
//                else
//                    _lineList.Clear();
//            }
//            else
//            {
//                if (_lineList.Count > 2)
//                {
//                    _comboList.AddRange(_lineList);
//                    _lineList.Clear();
//                }
//                else
//                    _lineList.Clear();
//            }
//        }
//        else
//        {
//            if (_lineList.Count > 2)
//            {
//                _comboList.AddRange(_lineList);
//                _lineList.Clear();
//            }
//            else
//                _lineList.Clear();
//        }
//    }

//    public my_active_cell_move[] MyIsThereACombo(int _x, int _y)
//    {
//        List<my_active_cell_move> comboList = new List<my_active_cell_move>();
//        List<my_active_cell_move> tempLineList = new List<my_active_cell_move>();

//        for (int y = 0; y < myArrCell.GetLength(1) - 1; y++)
//        {
//            my_active_cell_move activeCellFirst = myArrCell[_x, y].MyGetActiveCell;
//            my_active_cell_move activeCellSecond = myArrCell[_x, y + 1].MyGetActiveCell;

//            MyAddLineGemsCombo(activeCellFirst, activeCellSecond, ref comboList, ref tempLineList);

//            if (y + 1 == myArrCell.GetLength(1) - 1 && tempLineList.Count > 2)
//            {
//                comboList.AddRange(tempLineList);
//                tempLineList.Clear();
//            }
//        }

//        for (int x = 0; x < myArrCell.GetLength(0) - 1; x++)
//        {
//            my_active_cell_move activeCellFirst = myArrCell[x, _y].MyGetActiveCell;
//            my_active_cell_move activeCellSecond = myArrCell[x + 1, _y].MyGetActiveCell;

//            MyAddLineGemsCombo(activeCellFirst, activeCellSecond, ref comboList, ref tempLineList);

//            if (x + 1 == myArrCell.GetLength(0) - 1 && tempLineList.Count > 2)
//            {
//                comboList.AddRange(tempLineList);
//                tempLineList.Clear();
//            }
//        }

//        return comboList.ToArray();
//    }

//    ~MyTwoDArrayField()
//    {
//        myArrCell = null;
//    }
//}

public class my_main : MonoBehaviour
{
    private List<my_active_cell_move> myListCellActive;

    private List<my_source_gem> myListSourceGem;

    //private MyTwoDArrayField myArr;
    private my_cell_script[,] myArrCell;

    private Coroutine myCheckStateCoroutine;

    private TimeSpan myTimerState;
    private readonly TimeSpan myTimerStateEnd = new TimeSpan(0, 0, 0, 0, 300);

    private bool myIsAnimateReverseCell = false;

    public bool myIsHandle = true;
    public bool myIsAnimate = false;

    private void Start()
    {
        myListCellActive = new List<my_active_cell_move>();

        myListCellActive.AddRange(FindObjectsOfType<my_active_cell_move>());
        print("cell active count = " + myListCellActive.Count);

        MySortList mySort = new MySortList();
        myListCellActive.Sort(mySort);

        for (int ind = 0; ind < myListCellActive.Count; ind++)
        {
            myListCellActive[ind].MyInitializeColliders();
        }

        my_cell_script[] myArr = FindObjectsOfType<my_cell_script>();
        const int hei = 10, wid = 10;
        myArrCell = new my_cell_script[hei, wid];

        Array.Sort(myArr, new MySortArray());

        for (int y = 0; y < myArrCell.GetLength(1); y++)
        {
            for (int x = 0; x < myArrCell.GetLength(0); x++)
            {
                myArrCell[x, y] = myArr[(hei * y) + x];
            }
        }

        my_gem.MyEventEndPlayAnimation += My_gem_MyEventEndPlayAnimation;
        my_gem.MyEventEndPlayAnimationCell += My_gem_MyEventEndPlayAnimationCell;
        my_active_cell_move.MyEventMouseClickOnCell += My_active_cell_move_MyEventMouseClickOnCell;

        StartCoroutine(MyAfterStart());
    }


    private IEnumerator MyAfterStart()
    {
        yield return new WaitForSeconds(0.1f);

        myListSourceGem = new List<my_source_gem>();
        myListSourceGem.AddRange(FindObjectsOfType<my_source_gem>());

        MyCreateRandomGems();
        MyMoveDownGems();
    }

    private void MyCreateRandomGems()
    {
        //print("create random gems");
        myIsHandle = false;

        for (int indGem = 0; indGem < myListSourceGem.Count; indGem++)
        {
            myListSourceGem[indGem].MyAddGemRandom();
            //myListSourceGem[indGem].MyAddGemCalculated(ref myListCellActive);
        }
        //print("end create random gems");
    }

    private void MyAddLineGemsCombo(my_active_cell_move _first, my_active_cell_move _second,
        ref List<my_active_cell_move> _comboList, ref List<my_active_cell_move> _lineList)
    {
        //print("MyAddLineGemsCombo");
        if (_first != null && _second != null)
        {
            if (_first.MyIsHaveGem && _second.MyIsHaveGem)
            {
                //Debug.Log("combo  gem = " + _first.myGem);
                if (_lineList.Count == 0 && (_first.myGem.MyGetType == _second.myGem.MyGetType))
                {
                    _lineList.Add(_first);
                    _lineList.Add(_second);
                }
                else if (_lineList.Count > 0 && (_first.myGem.MyGetType == _second.myGem.MyGetType))
                {
                    _lineList.Add(_second);
                }
                else if (_lineList.Count > 2 && (_first.myGem.MyGetType != _second.myGem.MyGetType))
                {
                    _comboList.AddRange(_lineList);
                    _lineList.Clear();
                }
                else
                    _lineList.Clear();
            }
            else
            {
                if (_lineList.Count > 2)
                {
                    _comboList.AddRange(_lineList);
                    _lineList.Clear();
                }
                else
                    _lineList.Clear();
            }
        }
        else
        {
            if (_lineList.Count > 2)
            {
                _comboList.AddRange(_lineList);
                _lineList.Clear();
            }
            else
                _lineList.Clear();
        }
        //print("end MyAddLineGemsCombo");
    }

    private my_active_cell_move[] MyIsThereACombo(int _x, int _y)
    {
        //print("MyIsThereACombo");
        List<my_active_cell_move> comboList = new List<my_active_cell_move>();
        List<my_active_cell_move> tempLineList = new List<my_active_cell_move>();

        for (int y = 0; y < myArrCell.GetLength(1) - 1; y++)
        {
            my_active_cell_move activeCellFirst = myArrCell[_x, y].MyGetActiveCell;
            my_active_cell_move activeCellSecond = myArrCell[_x, y + 1].MyGetActiveCell;

            MyAddLineGemsCombo(activeCellFirst, activeCellSecond, ref comboList, ref tempLineList);

            if (y + 1 == myArrCell.GetLength(1) - 1 && tempLineList.Count > 2)
            {
                comboList.AddRange(tempLineList);
                tempLineList.Clear();
            }
        }

        for (int x = 0; x < myArrCell.GetLength(0) - 1; x++)
        {
            my_active_cell_move activeCellFirst = myArrCell[x, _y].MyGetActiveCell;
            my_active_cell_move activeCellSecond = myArrCell[x + 1, _y].MyGetActiveCell;

            MyAddLineGemsCombo(activeCellFirst, activeCellSecond, ref comboList, ref tempLineList);

            if (x + 1 == myArrCell.GetLength(0) - 1 && tempLineList.Count > 2)
            {
                comboList.AddRange(tempLineList);
                tempLineList.Clear();
            }
        }
        //print("end MyIsThereACombo");
        return comboList.ToArray();
    }

    private MyPairInt MyGetIndexCell(my_active_cell_move _cell)
    {
        //print("get index cell");
        MyPairInt pair;
        pair.x = -1;
        pair.y = -1;

        for (int y = 0; y < myArrCell.GetLength(1); y++)
        {
            for (int x = 0; x < myArrCell.GetLength(0); x++)
            {
                if (_cell.MyId == myArrCell[x, y].myId)
                {
                    pair.x = x;
                    pair.y = y;
                    print("end get index cell");
                    return pair;
                }
            }
        }
        //print("end wrong get index cell");
        return pair;
    }

    private void My_gem_MyEventEndPlayAnimationCell(my_active_cell_move _first, my_active_cell_move _target)
    {
        //print("event end play animation cell");
        if (myIsAnimateReverseCell)
        {
            myIsAnimateReverseCell = false;

            if (MySeeAllCombos().Length == 0)
            {
                _first.myGem.MyMoveToPointTheCell(_target);
                _target.myGem.MyMoveToPointTheCell(_first);
                myIsHandle = true;
            }
            else
            {
                My_gem_MyEventEndPlayAnimation();
            }
        }
        //print("end event end play animation cell");
    }

    private void MyCellOnExchange(my_active_cell_move _first, my_active_cell_move _target)
    {
        //print("cell on exchange");
        if (_target.myGem)
        {
            _first.myGem.MyMoveToPointTheCell(_target);
            _target.myGem.MyMoveToPointTheCell(_first);

            myIsAnimateReverseCell = true;
            myIsHandle = false;
        }
        //print("end cell on exchange");
    }

    private void My_active_cell_move_MyEventMouseClickOnCell(my_active_cell_move _cell, MyDirection _dir)
    {
        if (myIsHandle)
        {
            MyPairInt index;
            my_active_cell_move cellTarget = null;

            try
            {
                switch (_dir)
                {
                    case MyDirection.UNKNOWN:
                        break;
                    case MyDirection.LEFT:
                        index = MyGetIndexCell(_cell);
                        cellTarget = myArrCell[index.x - 1, index.y].MyGetActiveCell;

                        if (cellTarget)
                            MyCellOnExchange(_cell, cellTarget);
                        break;
                    case MyDirection.RIGHT:
                        index = MyGetIndexCell(_cell);
                        cellTarget = myArrCell[index.x + 1, index.y].MyGetActiveCell;

                        if (cellTarget)
                            MyCellOnExchange(_cell, cellTarget);
                        break;
                    case MyDirection.UP:
                        index = MyGetIndexCell(_cell);
                        cellTarget = myArrCell[index.x, index.y - 1].MyGetActiveCell;

                        if (cellTarget)
                            MyCellOnExchange(_cell, cellTarget);
                        break;
                    case MyDirection.DOWN:
                        index = MyGetIndexCell(_cell);
                        cellTarget = myArrCell[index.x, index.y + 1].MyGetActiveCell;

                        if (cellTarget)
                            MyCellOnExchange(_cell, cellTarget);
                        break;
                    default:
                        print("Unknow command direction");
                        break;
                }
            }
            catch(System.Exception _ex) { }
        }
    }

    private my_active_cell_move[] MySeeAllCombos()
    {
        //print("see all combo");
        List<my_active_cell_move> arrList = new List<my_active_cell_move>();
        for (int ind = 0; ind < 10; ind++)
        {
            arrList.AddRange(MyIsThereACombo(ind, ind));
        }
        //print("end see all combo");

        return arrList.ToArray();
    }

    private bool MyCheckIsAnimate()
    {
        //print("check is animate");
        for (int ind = 0; ind < myListCellActive.Count; ind++)
        {
            if (myListCellActive[ind].myGem && myListCellActive[ind].myGem.MyIsAnimate)
                return true;
        }
        //print("end check is animate");
        return false;
    }

    private void My_gem_MyEventEndPlayAnimation()
    {
        if (!MyCheckIsAnimate() && !myIsAnimate)
        {
            myIsAnimate = true;
            print("start animate");

            //if (myCheckStateCoroutine != null)
            //    StopCoroutine(myCheckStateCoroutine);

            //myCheckStateCoroutine = StartCoroutine(MyStartSelectState(0.1f));
        }
    }

    private IEnumerator MyStartSelectState(float _time)
    {
        print("select state");
        
        while (myIsAnimate)
        {
            yield return new WaitForSeconds(_time);

            if (MyIsCheckIfThePathIsFound())
            {
                print("move gems");
                //yield return new WaitForSeconds(0.1f);
                print("after move gems");
                MyMoveDownGems();
                print("after move gems");
            }
            else if (MySeeAllCombos().Length > 0)
            {
                print("Check combo");
                //yield return new WaitForSeconds(0.1f);
                MyIsCheckAndSetCombo();
            }
            else
            {
                print("end animated");
                myIsAnimate = false;
                myIsHandle = true;
            }
            print("while loop");
        }
        
        print("select state end");
    }

    private bool MyIsCheckAndSetCombo()
    {
        //print("check and set combo");
        my_active_cell_move[] myArrayCombo = MySeeAllCombos();

        for (int ind = 0; ind < myArrayCombo.Length; ind++)
        {
            myArrayCombo[ind].MyOnGemDestroy();
        }
        //print("end check and set combo");
        return myArrayCombo != null;
    }

    private bool MyIsCheckIfThePathIsFound()
    {
        //print("check if the path is found");
        for (int ind = 0; ind < myListCellActive.Count; ind++)
        {
            if (myListCellActive[ind].MyIsHaveAMove())
            {
                return true;
            }
        }
        //print("end check if the path is found");
        return false;
    }

    private void MyMoveDownGems()
    {
        MyTryMoveGemsDown();
    }

    private void MyTryMoveGemsDown()
    {
        //print("try move gems down");

        while (MyCheckTheCellIsDown())
        {
            MyCalculateAnimationListMove(MyParseCellDown());
            MyMoveDownGemsAnimation();
            MyCreateRandomGems();
        }

        while (MyCheckTheCellIsRightOrLeft())
        {
            MyCalculateAnimationListMove(MyParseCellLeftOrRight());
            MyMoveDownGemsAnimation();
            MyCreateRandomGems();
        }

        //print("end try move gems down");
    }

    private bool MyCheckTheCellIsDown()
    {
        //print("check the cell is down");
        for (int ind = 0; ind < myListCellActive.Count; ind++)
        {
            if (myListCellActive[ind].myGem &&
                myListCellActive[ind].MyCheckAnimationDirection(MyDirectionCell.MIDDLE))
                return true;
        }
        //print("end check the cell is down");
        return false;
    }

    private bool MyCheckTheCellIsRightOrLeft()
    {
        //print("check the cell is right or left");
        for (int ind = 0; ind < myListCellActive.Count; ind++)
        {
            if (myListCellActive[ind].myGem &&
                (myListCellActive[ind].MyCheckAnimationDirection(MyDirectionCell.LEFT) ||
                myListCellActive[ind].MyCheckAnimationDirection(MyDirectionCell.RIGHT)))
                return true;
        }
        //print("end check the cell is right or left");
        return false;
    }

    private my_active_cell_move[] MyParseCellLeftOrRight()
    {
        //print("parse cell left or right");
        List<my_active_cell_move> listCellDown = new List<my_active_cell_move>();

        for (int ind = 0; ind < myListCellActive.Count; ind++)
        {
            if (myListCellActive[ind].myGem && myListCellActive[ind].MyIsSelectedAnimate == false &&
                myListCellActive[ind].myGem.MyIsAnimate == false &&
                (myListCellActive[ind].MyCheckAnimationDirection(MyDirectionCell.LEFT) ||
                myListCellActive[ind].MyCheckAnimationDirection(MyDirectionCell.RIGHT)))
            {
                myListCellActive[ind].MyIsSelectedAnimate = true;
                listCellDown.Add(myListCellActive[ind]);

                //print("leftOrRight = " + myListCellActive[ind].transform.parent.name); 
            }
        }
        //print("end parse cell left or right");
        return listCellDown.ToArray();
    }

    private my_active_cell_move[] MyParseCellDown()
    {
        //print("parse cell down");
        List<my_active_cell_move> listCellDown = new List<my_active_cell_move>();

        for (int ind = 0; ind < myListCellActive.Count; ind++)
        {
            if (myListCellActive[ind].myGem && myListCellActive[ind].MyIsSelectedAnimate == false &&
                myListCellActive[ind].myGem.MyIsAnimate == false &&
                myListCellActive[ind].MyCheckAnimationDirection(MyDirectionCell.MIDDLE))
            {
                myListCellActive[ind].MyIsSelectedAnimate = true;
                listCellDown.Add(myListCellActive[ind]);

                //print("down = " + myListCellActive[ind].transform.parent.name);
            }
        }
        //print("end parse cell down");
        return listCellDown.ToArray();
    }

    private void MyMoveDownGemsAnimation()
    {
        //print("move down gems animation");
        for (int ind = myListCellActive.Count - 1; ind >= 0; ind--)
        {
            if (myListCellActive[ind].myGem && !myListCellActive[ind].myGem.MyIsAnimate)
            {
                myListCellActive[ind].MyTryToMoveGemDownV2();
            }
        }

        //print("end move down gems animation");
    }

    private void MyCalculateAnimationListMove(my_active_cell_move[] _arrCell)
    {
        //print("calculate animation list move");
        for (int ind = _arrCell.Length - 1; ind >= 0; ind--)
        {
            //print("cell gem = " + myListCellActive[ind].transform.parent.name);
            my_active_cell_move startCell = _arrCell[ind];
            startCell.myListMoveColliders.Clear();
            my_active_cell_move tempCell = null;

            my_active_cell_move resultDown = startCell.MyAddAnimationDown(startCell);
            my_active_cell_move resultLeft = _arrCell[ind].MyCheckAnimationDirection(MyDirectionCell.LEFT);
            my_active_cell_move resultRight = _arrCell[ind].MyCheckAnimationDirection(MyDirectionCell.RIGHT);

            int countBroken = 0;

            while (countBroken <= 1000)
            {
                countBroken++;

                if (resultDown)
                {
                    //print("resultDown while cell = " + resultDown.transform.parent.name);
                    tempCell = resultDown;
                    resultLeft = resultDown.MyCheckAnimationDirection(MyDirectionCell.LEFT);
                    resultRight = resultDown.MyCheckAnimationDirection(MyDirectionCell.RIGHT);
                    resultDown = resultDown.MyAddAnimationDown(startCell);

                    if (resultDown || resultLeft || resultRight)
                        tempCell.MyIsTestBusy = false;

                }
                else if (resultLeft)
                {
                    //print("resultLeft while cell = " + resultLeft.transform.parent.name);
                    startCell.MyIsTestBusy = false;
                    resultLeft.MyIsTestBusy = true;
                    tempCell = resultLeft;
                    startCell.myListMoveColliders.Add(resultLeft);
                    resultRight = resultLeft.MyCheckAnimationDirection(MyDirectionCell.RIGHT);
                    resultDown = resultLeft.MyAddAnimationDown(startCell);
                    resultLeft = resultLeft.MyCheckAnimationDirection(MyDirectionCell.LEFT);

                    if (resultDown || resultLeft || resultRight)
                        tempCell.MyIsTestBusy = false;
                }
                else if (resultRight)
                {
                    //print("resultRight while cell = " + resultRight.transform.parent.name);
                    startCell.MyIsTestBusy = false;
                    resultRight.MyIsTestBusy = true;
                    tempCell = resultRight;
                    startCell.myListMoveColliders.Add(resultRight);
                    resultLeft = resultRight.MyCheckAnimationDirection(MyDirectionCell.LEFT);
                    resultDown = resultRight.MyAddAnimationDown(startCell);
                    resultRight = resultRight.MyCheckAnimationDirection(MyDirectionCell.RIGHT);

                    if (resultDown || resultLeft || resultRight)
                        tempCell.MyIsTestBusy = false;
                }
                else
                {
                    break;
                }
                if (countBroken == 999)
                    print("broken while");
            }
            //print("end gem = " + myListCellActive[ind].transform.parent.name);
        }
        //print("end calculate animation list move");
    }

    private void OnDisable()
    {
        my_gem.MyEventEndPlayAnimation -= My_gem_MyEventEndPlayAnimation;
        my_gem.MyEventEndPlayAnimationCell -= My_gem_MyEventEndPlayAnimationCell;
        my_active_cell_move.MyEventMouseClickOnCell -= My_active_cell_move_MyEventMouseClickOnCell;
    }

    private void FixedUpdate()
    {
        if (myIsAnimate)
        {
            myTimerState += TimeSpan.FromSeconds(Time.deltaTime);
            if (myTimerState < myTimerStateEnd)
                return;

            my_active_cell_move[] myTest = MySeeAllCombos();
            print("all combo = " + myTest);

            if (MyIsCheckIfThePathIsFound())
            {
                print("move gems");
                //yield return new WaitForSeconds(0.1f);
                MyMoveDownGems();
            }
            else if (myTest != null && myTest.Length > 0)
            {
                print("Check combo");
                //yield return new WaitForSeconds(0.1f);
                MyIsCheckAndSetCombo();
            }
            else
            {
                print("end animated");
                myIsAnimate = false;
                myIsHandle = true;
            }

            print("while loop");
            myTimerState = TimeSpan.Zero;
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
            MyIsCheckAndSetCombo();

        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (myIsHandle)
        {
            
        }
    }
}
