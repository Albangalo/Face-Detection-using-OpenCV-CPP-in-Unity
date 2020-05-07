using System;
using UnityEngine;

public class PositionAtFaceScreenSpace : MonoBehaviour
{
    private float _camDistance;
    private float[] windowX;
    private float[] windowY;
    private float[] windowZ;
    public int WINDOW_SIZE = 5;
    public float X_SCALE = 2f;
    public float Y_SCALE = 2f;
    public float Z_SCALE = 0.5f;
    public float X_OFFSET = -0.42f;
    public float Y_OFFSET = -0.2f;
    public float Z_OFFSET = 0f;
    private Vector3 HeadPos;
    private Assets.HolographicDisplay.SimpleHolographicCamera HoloCamScript;

    void Start()
    {
        _camDistance = Vector3.Distance(Camera.main.transform.position, transform.position);
        HeadPos = Vector3.zero;
        windowX = new float[WINDOW_SIZE];
        windowY = new float[WINDOW_SIZE];
        windowZ = new float[WINDOW_SIZE];
        for (int i = 0; i < WINDOW_SIZE; i++)
        {
            windowX[i] = 0;
            windowY[i] = 0;
            windowZ[i] = 0;
        }
    }

    void Update()
    {
        if (OpenCVFaceDetection.NormalizedFacePositions.Count == 0)
            return;

        var currX = OpenCVFaceDetection.NormalizedFacePositions[0].x;
        var currY = OpenCVFaceDetection.NormalizedFacePositions[0].y;
        var currZ = OpenCVFaceDetection.NormalizedFacePositions[0].z;

        UpdateWindow(windowX, currX);
        UpdateWindow(windowY, currY);
        UpdateWindow(windowZ, currZ);
 
        var smoothX = Median(windowX);
        var smoothY = Median(windowY);
        var smoothZ = Median(windowZ);

        print("currX:"+ currX+ ", currY:" + currY + ", currZ:" + currZ + ", smoothX: "+ smoothX + ", smoothY: " + smoothY + ", smoothZ: " + smoothZ);
        HeadPos = new Vector3((smoothX+X_OFFSET)*-X_SCALE, (smoothY+Y_OFFSET)*Y_SCALE, (smoothZ+Z_OFFSET)*Z_SCALE);
    }

    public Vector3 GetHeadPos()
    {
        return HeadPos;
    }

    private float Median(float[] vals)
    {
        var temp = (float[])vals.Clone();
        return select(temp, 0, temp.Length - 1, temp.Length / 2);
    }

    private void UpdateWindow(float[] window, float value)
    {
        for (int i = window.Length-2; i >= 0; i--)
        {
            window[i + 1] = window[i];
        }
        window[0] = value;
    }

    private int partition(float[] nums, int left, int right, int pivotIndex)
    {
        float pivotVal = nums[pivotIndex];
        swap(nums, pivotIndex, right);
        int storeIndex = left;
        for (int i = left; i < right; i++)
        {
            if (nums[i] < pivotVal)
            {
                swap(nums, i, storeIndex);
                storeIndex++;
            }
        }
        swap(nums, storeIndex, right);
        return storeIndex;
    }

    private float select(float[] nums, int left, int right, int k)
    {
        if (left == right)
            return nums[left];
        int pivotIndex = UnityEngine.Random.Range(left, right + 1);
        pivotIndex = partition(nums, left, right, pivotIndex);

        if (pivotIndex == k)
            return nums[pivotIndex];
        else if (pivotIndex < k)
            return select(nums, pivotIndex + 1, right, k);
        else
            return select(nums, left, pivotIndex - 1, k);
    }

    private void swap(float[] nums, int a, int b)
    {
        float temp = nums[a];
        nums[a] = nums[b];
        nums[b] = temp;
    }
}
