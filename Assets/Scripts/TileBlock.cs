using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class TileBlock : MonoBehaviour
{
    public List<Tile> tiles;
    public LayerMask gridMask, hitMask;
    public SortingGroup sortingGroup;
    
    private bool holding;
    private Vector3 prevPos;
    private float checkRadius = 0.3f;
    private Vector3 offset;
    private Hand theHand;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        prevPos = transform.position;
    }

    private void Update()
    {
        if(holding)
        {
            var mp = Input.mousePosition;
            mp.z = 10f;
            var mouseInWorld = cam.ScreenToWorldPoint(mp);

            transform.position = mouseInWorld + offset;

            HoverOut();
            CheckValidity();
            CheckBlocking();
        }
    }

    public void Setup(Hand hand)
    {
        theHand = hand;
    }

    public void Drop()
    {
        holding = false;
        SetOutlineColor(Color.white);

        var blocked = CheckBlocking(false);

        if (blocked)
        {
            Tweener.Instance.MoveTo(transform, prevPos, 0.15f, 0f, TweenEasings.BounceEaseOut);
            Invoke("AfterMouseUp", 0.15f);

            return;
        }

        var ok = CheckValidity(false);

        if(ok)
        {
            tiles.ForEach(tile => tile.gameObject.layer = 7);
            
            var gridPos = Physics2D.OverlapCircleAll(transform.position, checkRadius, gridMask);
            var p = transform.position;
            var snapPos = new Vector3(Mathf.Round(p.x * 2f) * 0.5f, Mathf.Round(p.y * 2f) * 0.5f, Mathf.Round(p.z));

            Tweener.Instance.MoveTo(transform, snapPos, 0.12f, 0f, TweenEasings.BounceEaseOut);
            
            theHand.Spawn();
            
            Invoke("AfterMouseUp", 0.12f);
            return;
        }
        
        prevPos = transform.localPosition;

        AfterMouseUp();
    }

    private void SetOutlineColor(Color color)
    {
        tiles.ForEach(tile => tile.focus.color = color);
    }

    private void AfterMouseUp()
    {
        sortingGroup.sortingOrder = 0;
    }

    public void HoverOut()
    {
        SetOutlineColor(Color.clear);
    }

    public void HoverIn()
    {
        SetOutlineColor(Color.white);
    }

    public void Grab()
    {
        holding = true;
        HoverOut();

        var mp = Input.mousePosition;
        mp.z = 10f;
        var mouseInWorld = cam.ScreenToWorldPoint(mp);

        offset = transform.position - mouseInWorld;
    }

    private bool CheckBlocking(bool color = true)
    {
        var blocked = false;
        var onGrid = 0;
        var offGrid = 0;

        tiles.ForEach(tile =>
        {
            if (tile.isDecoration)
                return;

            var gridHits = Physics2D.OverlapCircleAll(tile.transform.position, checkRadius, gridMask);
            var hits = Physics2D.OverlapCircleAll(tile.transform.position, checkRadius, hitMask);

            if (hits.Length > 0)
                blocked = true;

            if (gridHits.Length == 0)
                offGrid++;

            if (gridHits.Length > 0)
                onGrid++;
        });

        var ret = blocked || onGrid > 0 && offGrid > 0;

        if (ret && color)
            SetOutlineColor(Color.red);

        return ret;
    }

    private bool CheckValidity(bool color = true)
    {
        var ok = true;

        tiles.ForEach(tile =>
        {
            if (tile.isDecoration)
                return;
            
            var gridHits = Physics2D.OverlapCircleAll(tile.transform.position, checkRadius, gridMask);

            if (gridHits.Length == 0)
                ok = false;
        });

        if(ok && color)
            SetOutlineColor(Color.green);

        return ok;
    }
}