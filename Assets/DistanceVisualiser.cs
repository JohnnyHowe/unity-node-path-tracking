using UnityEngine;
using TMPro;

public class DistanceVisualiser : MonoBehaviour
{
    [SerializeField] private NodePathDistanceTracker _nodePathDistanceTracker;
    [SerializeField] private TextMeshProUGUI _text;

    void Update()
    {
        _text.transform.parent.transform.LookAt(Camera.main.transform);
        float distance = _nodePathDistanceTracker.GetT(transform.position);
        string distanceString = distance.ToString();
        _text.text = distanceString.Substring(0, Mathf.Min(5, distanceString.Length));
    }
}
