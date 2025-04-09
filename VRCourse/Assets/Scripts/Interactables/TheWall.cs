using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using static OVRPlugin;

[ExecuteAlways]
public class TheWall : MonoBehaviour
{
    public UnityEvent OnDestroy;

    [SerializeField]
    int columns;

    [SerializeField]
    int rows;

    [SerializeField]
    GameObject wallCubePrefab;

    [SerializeField]
    GameObject socketWallCubePrefab;

    [SerializeField]
    int socketWallPosition = 1;

    [SerializeField]
    List<GeneratedColumn> generatedColumn;

    [SerializeField]
    float cubeSpacing;

    [SerializeField]
    bool buildWall;

    [SerializeField]
    bool deleteWall;

    [SerializeField]
    bool destroyWall;

    [SerializeField]
    int maxPower = 2000;

    [SerializeField]
    AudioClip destoryWallClip;

    public AudioClip GetDestroyClip => destoryWallClip;

    [SerializeField]
    AudioClip socketClip;
    public AudioClip GetSocketClip => socketClip;

    Vector3 cubeSize;
    Vector3 spawnPos;
    GameObject[] wallCubes;
    [SerializeField]
    XRSocketInteractor wallSocket;
    public XRSocketInteractor GetWallSocket => wallSocket;

    [SerializeField]
    ExplosiveDevice explosiveDevice;

    void Start()
    {
        if (wallSocket != null)
        {
            wallSocket.selectEntered.AddListener(WallSocket_OnSelectEntered);
            wallSocket.selectExited.AddListener(WallSocket_OnSelectExited);
        }

        if (explosiveDevice != null)
        {
            explosiveDevice.OnDetonated.AddListener(OnDetonated_OnDestroyWall);
        }
    }

    void BuildWall()
    {
        if (wallCubePrefab != null)
        {
            cubeSize = wallCubePrefab.GetComponent<Renderer>().bounds.size;
        }

        spawnPos = transform.position;
        int socketedColumn = Random.Range(0, columns);

        for (int i = 0; i < columns; i++)
        {
            if (i == socketedColumn)
            {
                GenerateColumn(i, rows, true);
            }
            else
            {
                GenerateColumn(i, rows, false);
            }
            
            spawnPos.x += cubeSize.x + cubeSpacing;
        }
    }

    private void WallSocket_OnSelectEntered(SelectEnterEventArgs arg0)
    {
        if (generatedColumn.Count >= 1)
        {
            for (int i = 0; i < generatedColumn.Count; i++)
            {
                generatedColumn[i].ActivateColumn();
            }
        }
    }

    void OnDetonated_OnDestroyWall()
    {
        int power;

        if (generatedColumn.Count >= 1)
        {
            for (int i = 0; i < generatedColumn.Count; i++)
            {
                power = Random.Range(maxPower / 2, maxPower);
                generatedColumn[i].DestroyColumn(power);
            }
        }

        OnDestroy?.Invoke();
    }

    private void WallSocket_OnSelectExited(SelectExitEventArgs arg0)
    {
        if (generatedColumn.Count >= 1)
        {
            for (int i = 0; i < generatedColumn.Count; i++)
            {
                generatedColumn[i].ResetColumn();
            }
        }
    }

    void AddSocketWall(GeneratedColumn socketedColumn)
    {
        if (wallCubes[socketWallPosition] != null)
        {
            Vector3 pos = wallCubes[socketWallPosition].transform.position;
            DestroyImmediate(wallCubes[socketWallPosition]);

            wallCubes[socketWallPosition] = Instantiate(socketWallCubePrefab, pos, transform.rotation);
            socketedColumn.SetCube(wallCubes[socketWallPosition]);

            if (socketWallPosition == 0)
            {
                wallCubes[socketWallPosition].transform.SetParent(transform);
            }
            else
            {
                wallCubes[socketWallPosition].transform.SetParent(wallCubes[0].transform);
            }

            wallSocket = wallCubes[socketWallPosition].GetComponentInChildren<XRSocketInteractor>();

            if (wallSocket != null)
            {
                wallSocket.selectEntered.AddListener(WallSocket_OnSelectEntered);
                wallSocket.selectExited.AddListener(WallSocket_OnSelectExited);
            }
        }
    }

    void GenerateColumn(int index, int height, bool socketable)
    {
        GeneratedColumn tempColumn = new GeneratedColumn();
        tempColumn.InitializeColumn(transform, index, height, socketable);

        spawnPos.y = transform.position.y;
        wallCubes = new GameObject[height];

        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubePrefab != null)
            {
                wallCubes[i] = Instantiate(wallCubePrefab, spawnPos, transform.rotation);
                tempColumn.SetCube(wallCubes[i]);
            }
            
            spawnPos.y += cubeSize.y + cubeSpacing;
        }

        if (socketable && socketWallCubePrefab != null)
        {
            if (socketWallPosition < 0 || socketWallPosition >= height)
            {
                socketWallPosition = 0;
            }

            AddSocketWall(tempColumn);
        }

        generatedColumn.Add(tempColumn);
    }

    private void SetObjectToKinematic(GameObject obj, bool enableKinematic)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.isKinematic = enableKinematic;
    }

    private void Update()
    {
        if (buildWall)
        {
            buildWall = false;
            BuildWall();
        }

        if (deleteWall)
        {
            deleteWall = false;
            for (int i = 0; i < generatedColumn.Count; i++)
             {
                generatedColumn[i].DeleteColumn();
             }
            
            if (generatedColumn.Count >= 1)
            {
                generatedColumn.Clear();
            }
        }
    }
}

[System.Serializable]
public class GeneratedColumn
{
    [SerializeField]
    GameObject[] wallCubes;

    [SerializeField]
    bool isSocketed;

    [SerializeField]
    int columnIndex;

    Transform parentObj;

    const string COLUMN_NAME = "column";
    const string SOCKETED_COLUMN_NAME = "socketedColumn";
    bool isParented;
    Transform columnObj;

    public void InitializeColumn(Transform parent, int index, int rows, bool socketed)
    {
        parentObj = parent;
        columnIndex = index;
        wallCubes = new GameObject[rows];
        isSocketed = socketed;
    }

    public void SetCube(GameObject cube)
    {

        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (!isParented)
            {
                isParented = true;
                SetColumnName(cube, columnIndex);
                cube.transform.SetParent(parentObj);
                columnObj = cube.transform;
            }
            else
            {
                cube.transform.SetParent(columnObj);
            }

            if (wallCubes[i] == null)
            {
                wallCubes[i] = cube;
                break;
            }
        }
    }

    void SetColumnName(GameObject column, int index)
    {
        if (isSocketed)
        {
            column.name = SOCKETED_COLUMN_NAME;
        }
        else
        {
            column.name = COLUMN_NAME;
        }

        column.name += index.ToString();
    }

    public void DeleteColumn()
    {
        for (int i = 0; i < wallCubes.Length; ++i)
        {
            if (wallCubes[i] != null)
            {
                Object.DestroyImmediate(wallCubes[i]);
            }
        }

        wallCubes = new GameObject[0];
    }

    public void DestroyColumn(int power)
    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubes[i] != null)
            {
                Rigidbody rb = wallCubes[i].GetComponent<Rigidbody>();
                rb.isKinematic = false;

                rb.constraints = RigidbodyConstraints.None;
                wallCubes[i].transform.SetParent(parentObj);

                rb.AddRelativeForce(Random.onUnitSphere * power);
            }
        }
    }

    public void ActivateColumn()
    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubes[i] != null)
            {
                Rigidbody rb = wallCubes[i].GetComponent<Rigidbody>();
                rb.isKinematic = false;
            }
        }
    }

    public void ResetColumn()
    {
        for (int i = 0; i < wallCubes.Length; i++)
        {
            if (wallCubes[i] != null)
            {
                Rigidbody rb = wallCubes[i].GetComponent<Rigidbody>();
                rb.isKinematic = false;
            }
        }
    }
}
