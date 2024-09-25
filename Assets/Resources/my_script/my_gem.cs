using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class my_gem : MonoBehaviour
{
    [SerializeField, Range(1.0f, 2000.0f)]
    private float my_speed_gem_move;

    public bool MyIsAnimate;

    private my_gem_type[] myGemType;

    private MyTypeGem myType;

    private Coroutine myMovePointToCellCoroutine;

    private Vector3 startPos;
    private float startTime;
    private float distLength;
    private int index;

    private TimeSpan myTimerState;
    private readonly TimeSpan myTimerStateEnd = new TimeSpan(0, 0, 0, 0, 500);

    public MyTypeGem MyGetType { get { return myType; } }

    public List<my_active_cell_move> myPathAnimation;

    //public static bool MyIsAnimateStatic = false;
    public static event System.Action MyEventEndPlayAnimation;
    public static event System.Action<my_active_cell_move, my_active_cell_move> MyEventEndPlayAnimationCell;

    private void Awake()
    {
        myPathAnimation = new List<my_active_cell_move>();
        MyIsAnimate = false;

        myGemType = new my_gem_type[6];
        myGemType[0] = Resources.Load<my_gem_type>("my_prefab/my_gem_romb");
        myGemType[1] = Resources.Load<my_gem_type>("my_prefab/my_gem_rect");
        myGemType[2] = Resources.Load<my_gem_type>("my_prefab/my_gem_oval");
        myGemType[3] = Resources.Load<my_gem_type>("my_prefab/my_gem_circle");
        myGemType[4] = Resources.Load<my_gem_type>("my_prefab/my_gem_octagon");
        myGemType[5] = Resources.Load<my_gem_type>("my_prefab/my_gem_triangle");

    }

    public void MySpawnGem(MyTypeGem _type)
    {
        //print("gem MySpawnGem");
        my_gem_type go = Instantiate(myGemType[(int)_type]);
        go.transform.SetParent(transform, true);

        myType = _type;
        //print("end gem MySpawnGem");
    }

    private void OnDestroy()
    {
        //print("gem OnDestroy");
        if (this != null)
            transform.parent.GetComponent<my_active_cell_move>().MyOnGemDestroy();
        //print("end gem OnDestroy");
    }

    public void MyTryMoveDownV2(my_active_cell_move _parentActiveCell)
    {
        //print("gem MyTryMoveDownV2");
        myPathAnimation.Clear();
        myPathAnimation.AddRange(_parentActiveCell.myListMoveColliders);
        _parentActiveCell.myListMoveColliders.Clear();
        _parentActiveCell.MyIsSelectedAnimate = false;

        if (myPathAnimation.Count <= 0)
        {
            //print("end break gem MyTryMoveDownV2");
            return;
        }
        
        my_active_cell_move lastHelper = myPathAnimation[myPathAnimation.Count - 1];

        if (lastHelper)
        {
            //if (myMoveDownCoroutine != null)
            //    StopCoroutine(myMoveDownCoroutine);
            //print("gem MyTryMoveDownV2 move");
            lastHelper.MySetBusy(true);
            //myMoveDownCoroutine = StartCoroutine(MyMovingByPoints(0.1f, _parentActiveCell));
            MyMovingByPoints(_parentActiveCell);
        }
        //print("end gem MyTryMoveDownV2");
    }

    public void MyMoveToPointTheCell(my_active_cell_move _targetCell)
    {
        //print("gem MyMoveToPointTheCell");
        if (myMovePointToCellCoroutine != null)
            StopCoroutine(myMovePointToCellCoroutine);

        myMovePointToCellCoroutine = StartCoroutine(MyMovingToPointTheCell(0.01f, _targetCell));
        //print("end gem MyMoveToPointTheCell");
    }

    private IEnumerator MyMovingToPointTheCell(float _timeUpdate, my_active_cell_move _targetCell)
    {
        print("gem MyMovingToPointTheCell");
        my_active_cell_move parentCell = transform.GetComponentInParent<my_active_cell_move>();
        MyIsAnimate = true;
        
        transform.SetParent(_targetCell.transform, true);

        Vector3 startPos = transform.position;
        float startTime = Time.time;
        float distLength = Vector3.Distance(startPos, _targetCell.transform.position);

        while (transform.position != _targetCell.transform.position)
        {
            float distCovered = (Time.time - startTime) * 500;
            float fracJorney = distCovered / distLength;
            Vector3 posPoint = _targetCell.transform.position;

            transform.position = Vector3.Lerp(startPos, posPoint, fracJorney);

            yield return new WaitForSeconds(_timeUpdate);
        }

        _targetCell.myGem = this;

        MyIsAnimate = false;

        yield return new WaitForSeconds(0.15f);

        MyEventEndPlayAnimationCell?.Invoke(parentCell, _targetCell);
        print("end gem MyMovingToPointTheCell");
    }

    //private IEnumerator MyMovingByPoints(float _timeUpdate, my_active_cell_move _parentCell)
    //{
    //    print("gem MyMovingByPoints");
    //    MyIsAnimate = true;
    //    my_active_cell_move lastCell = myPathAnimation[myPathAnimation.Count - 1];
    //    _parentCell.MySetBusy(false);
    //    lastCell.myGem = this;
    //    transform.SetParent(lastCell.transform, true);

    //    for (int ind = 0; ind < myPathAnimation.Count; ind++)
    //    {
    //        Vector3 startPos = transform.position;
    //        float startTime = Time.time;
    //        float distLength = Vector3.Distance(startPos, myPathAnimation[ind].transform.position);

    //        int brokenCounter = 0;

    //        while (brokenCounter <= 1000 && transform.position != myPathAnimation[ind].transform.position)
    //        {
    //            brokenCounter++;
    //            float distCovered = (Time.time - startTime) * my_speed_gem_move;
    //            float fracJorney = distCovered / distLength;
    //            Vector3 posPoint = myPathAnimation[ind].transform.position;

    //            transform.position = Vector3.Lerp(startPos, posPoint, fracJorney);

    //            yield return new WaitForSeconds(_timeUpdate);

    //            if (brokenCounter == 999)
    //                print("exeption my_gem brokenCounter = " + brokenCounter);
    //        }
    //    }

    //    myPathAnimation.Clear();
    //    MyIsAnimate = false;

    //    yield return new WaitForSeconds(0.15f);

    //    MyEventEndPlayAnimation?.Invoke();
    //    print("end gem MyMovingByPoints");
    //}

    public bool MyDoIHaveAMove(my_active_cell_move _source)
    {
        if (MyChoseTheDirection(_source))
            return true;

        return false;
    }

    private my_active_cell_move MyChoseTheDirection(my_active_cell_move _curr_cell)
    {
        //print("gem MyChoseTheDirection");
        if (_curr_cell.myActiveCellMoveCollider[1] && !_curr_cell.myActiveCellMoveCollider[1].MyIsBusyCell)
        {
            //print("end gem MyChoseTheDirection");
            return _curr_cell.myActiveCellMoveCollider[1];
        }
        else if (_curr_cell.myActiveCellMoveCollider[0] && !_curr_cell.myActiveCellMoveCollider[0].MyIsBusyCell)
        {
            //print("end gem MyChoseTheDirection");
            return _curr_cell.myActiveCellMoveCollider[0];
        }
        else if (_curr_cell.myActiveCellMoveCollider[2] && !_curr_cell.myActiveCellMoveCollider[2].MyIsBusyCell)
        {
            //print("end gem MyChoseTheDirection");
            return _curr_cell.myActiveCellMoveCollider[2];
        }

        //print("end null gem MyChoseTheDirection");
        return null;
    }

    private void MyMovingByPoints(my_active_cell_move _parentCell)
    {
        //print("gem MyMovingByPoints");
        my_active_cell_move lastCell = myPathAnimation[myPathAnimation.Count - 1];
        MyIsAnimate = true;
        _parentCell.MySetBusy(false);
        lastCell.myGem = this;
        transform.SetParent(lastCell.transform, true);
        transform.position = lastCell.transform.position;
        index = 0;
        myTimerState = TimeSpan.Zero;
        MyIsAnimate = false;

        MyEventEndPlayAnimation?.Invoke();
    }

    private void MyMoveGemToUpdate()
    {
        if (MyIsAnimate)
        {
            myTimerState += TimeSpan.FromSeconds(Time.deltaTime);
            if (myTimerState < myTimerStateEnd)
                return;

            if (myPathAnimation.Count == (index + 1))
            {
                print("end Animation timer");
                MyIsAnimate = false;
                myPathAnimation.Clear();
                MyEventEndPlayAnimation?.Invoke();
                return;
            }

            startPos = transform.position;
            float distCovered = (Time.time - startTime) * my_speed_gem_move;
            float fracJorney = distCovered / distLength;
            Vector3 posPoint = myPathAnimation[index].transform.position;

            transform.position = Vector3.Lerp(startPos, posPoint, fracJorney);

            //print("distance = " + Vector3.Distance(startPos, posPoint));
            //print("start pos = " + startPos + " end pos = " + posPoint);

            if (Vector3.Distance(startPos, posPoint) == 0)
            {
                index++;
                //print("distance = " + Vector3.Distance(startPos, posPoint));

                startTime = Time.time;
                distLength = Vector3.Distance(startPos, myPathAnimation[index].transform.position);
            }
            myTimerState = TimeSpan.Zero;
        }
    }

    private void FixedUpdate()
    {
        //MyMoveGemToUpdate();
    }
}
