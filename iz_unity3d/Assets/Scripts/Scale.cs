using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scale : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] Transform _obj;
    [SerializeField] float _minScale = 1f;
    [SerializeField] float _maxScale = 10f;
    [SerializeField] float _koef = 0.1f;

    Plane _plane;
    Transform _cameraTransform;

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
	bool 		        _twoTouchesCoroutine = false;
#endif

    void Start()
    {
        _plane = new Plane(Vector3.up, Vector3.up * 150);
        _cameraTransform = _camera.gameObject.transform;
    }

    void Update()
    {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)

		if (!_twoTouchesCoroutine && two_touches)
		{
			StartCoroutine(TwoTouches());
			return;
		}
#endif
    }

    private void OnDisable()
    {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        StopAllCoroutines();
        _twoTouchesCoroutine = false;
#endif
    }

    float Projection(Vector3 to, Vector3 from) //проекиця вектора from на ось to
    {
        return (to.x * from.x + to.y * from.y + to.z * from.z) / to.magnitude;
    }

    public static float AngleSigned(Vector3 from, Vector3 to) //угол между векторами -180...180
    {
        Vector3 norm = new Vector3(-from.z, 0, from.x);
        int sgn = Vector3.Angle(norm, to) < 90f ? 1 : -1;
        return sgn * Vector3.Angle(from, to);
    }


#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
    public static bool two_touches
	{
		get
		{
            return Input.touchCount == 2;
		}
	}

	IEnumerator TwoTouches()
	{
		_twoTouchesCoroutine = true;

        Ray ray = _camera.ScreenPointToRay(TouchPosition(0));
        float distance;
        _plane.Raycast(ray, out distance);
        Vector3 controlPoint = ray.GetPoint(distance);

        ray = _camera.ScreenPointToRay(TouchPosition(1));        
        _plane.Raycast(ray, out distance);
        Vector3 controlPoint1 = ray.GetPoint(distance);

        float deltaControlPoints = (controlPoint1 - controlPoint).magnitude;

        yield return null;

        while (two_touches)
		{
            ray = _camera.ScreenPointToRay(TouchPosition(0));            
            _plane.Raycast(ray, out distance);
            controlPoint = ray.GetPoint(distance);

            ray = _camera.ScreenPointToRay(TouchPosition(1));
            _plane.Raycast(ray, out distance);
            controlPoint1 = ray.GetPoint(distance);

            float newDeltaControlPoints = (controlPoint1 - controlPoint).magnitude;

            float scale = _obj.localScale.x + (newDeltaControlPoints - deltaControlPoints) * _koef;
            scale = Mathf.Clamp(scale, _minScale, _maxScale);
            _obj.transform.localScale = new Vector3(scale, scale, scale);

            deltaControlPoints = newDeltaControlPoints;
            yield return null;
		}

		_twoTouchesCoroutine = false;
	}

	Vector3 TouchPosition(int index)
	{
		if (index < Input.touchCount)
		{
			Touch touch = Input.GetTouch(index);
			return new Vector3(touch.position.x, touch.position.y, 0);
		}
		else
		{
			return Vector3.zero;
		}
	}

#endif
}
