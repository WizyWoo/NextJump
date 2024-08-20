using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform Target;
    public float Speed;
    public int YChange;
    public int YOffsetPositive, YOffsetNegative;
    public Vector3 _targetPos;

    private void Start()
    {
        _targetPos = transform.position;
    }

    private void Update()
    {
 
        if(Target.position.y > _targetPos.y + YOffsetPositive)
            _targetPos += new Vector3(0, YChange, 0);
        else if(Target.position.y < _targetPos.y - YOffsetNegative)
            _targetPos -= new Vector3(0, YChange, 0);

        transform.position = Vector3.Lerp(transform.position, _targetPos, Speed * Time.deltaTime);

    }

}
