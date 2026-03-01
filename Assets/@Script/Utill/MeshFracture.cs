using System.Collections.Generic;
using UnityEngine;

public class MeshFracture : MonoBehaviour
{
    [Header("Fracture Settings")]
    public float clusterDistance = 0.2f;      // 파편 묶는 거리
    public float explosionForce = 4f;         // 파편 튀기는 힘
    public float upwardModifier = 0.3f;       // 위로 튀는 정도
    public bool destroyOriginal = true;       // 원본 제거 여부
    public bool debugLog = false;

    private Mesh originalMesh;
    private Vector3[] verts;
    private int[] tris;

    // ---------------- Triangle 구조체 ----------------
    public class TriangleData
    {
        public Vector3[] verts = new Vector3[3];
        public Vector3 centroid;
        public int id;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Fracture();
        }
    }

    // ---------------- 실행 함수 ----------------
    public void Fracture()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null)
        {
            Debug.LogError("MeshFilter 없음!");
            return;
        }

        originalMesh = mf.mesh;
        verts = originalMesh.vertices;
        tris = originalMesh.triangles;

        // 1) 삼각형 데이터 만들기
        List<TriangleData> triangles = BuildTriangleData();

        // 2) 삼각형 클러스터링
        List<List<TriangleData>> clusters = ClusterTriangles(triangles);

        // 3) 클러스터를 파편으로 생성
        foreach (var cluster in clusters)
        {
            CreateShard(cluster);
        }

        // 4) 원본 제거
        if (destroyOriginal)
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<MeshFilter>().mesh = null;
        }

        if (debugLog)
            Debug.Log("Fracture Complete: " + clusters.Count + " shards created");
    }

    // ----------------------------------------------------
    // STEP 1. TriangleData 리스트 만들기
    // ----------------------------------------------------
    private List<TriangleData> BuildTriangleData()
    {
        List<TriangleData> triangles = new List<TriangleData>();

        for (int i = 0; i < tris.Length; i += 3)
        {
            TriangleData t = new TriangleData();

            // 삼각형 3개 정점을 로컬 좌표로 저장
            t.verts[0] = verts[tris[i]];
            t.verts[1] = verts[tris[i + 1]];
            t.verts[2] = verts[tris[i + 2]];

            // centroid (중심점)
            t.centroid = (t.verts[0] + t.verts[1] + t.verts[2]) / 3f;
            t.id = i / 3;

            triangles.Add(t);
        }

        return triangles;
    }

    // ----------------------------------------------------
    // STEP 2. BFS로 삼각형 클러스터링
    // ----------------------------------------------------
    private List<List<TriangleData>> ClusterTriangles(List<TriangleData> triangles)
    {
        List<List<TriangleData>> clusters = new List<List<TriangleData>>();
        bool[] visited = new bool[triangles.Count];

        for (int i = 0; i < triangles.Count; i++)
        {
            if (visited[i]) continue;

            Queue<int> queue = new Queue<int>();
            List<TriangleData> cluster = new List<TriangleData>();

            queue.Enqueue(i);
            visited[i] = true;

            while (queue.Count > 0)
            {
                int cur = queue.Dequeue();
                TriangleData curTri = triangles[cur];
                cluster.Add(curTri);

                for (int j = 0; j < triangles.Count; j++)
                {
                    if (visited[j]) continue;

                    float dist = Vector3.Distance(curTri.centroid, triangles[j].centroid);
                    if (dist < clusterDistance)
                    {
                        visited[j] = true;
                        queue.Enqueue(j);
                    }
                }
            }

            clusters.Add(cluster);
        }

        return clusters;
    }

    // ----------------------------------------------------
    // STEP 3. 클러스터 → Shard Mesh 생성
    // ----------------------------------------------------
    private void CreateShard(List<TriangleData> cluster)
    {
        List<Vector3> vertList = new List<Vector3>();
        List<int> triList = new List<int>();

        // centroid (로컬 공간)
        Vector3 centroidLocal = Vector3.zero;

        foreach (var tri in cluster)
            centroidLocal += tri.centroid;

        centroidLocal /= cluster.Count;

        // vertex 생성 (로컬 공간)
        for (int i = 0; i < cluster.Count; i++)
        {
            int baseIndex = vertList.Count;

            vertList.Add(cluster[i].verts[0]);
            vertList.Add(cluster[i].verts[1]);
            vertList.Add(cluster[i].verts[2]);

            triList.Add(baseIndex);
            triList.Add(baseIndex + 1);
            triList.Add(baseIndex + 2);
        }

        // Mesh 설정
        Mesh shardMesh = new Mesh();
        shardMesh.SetVertices(vertList);
        shardMesh.SetTriangles(triList, 0);
        shardMesh.RecalculateNormals();
        shardMesh.RecalculateBounds();

        // 월드 좌표의 centroid 구하기
        Vector3 centroidWorld = transform.TransformPoint(centroidLocal);

        // Shard GameObject 생성
        GameObject shard = new GameObject("Shard");
        shard.transform.position = centroidWorld;
        shard.transform.rotation = transform.rotation;
        shard.transform.localScale = transform.localScale;

        MeshFilter mf = shard.AddComponent<MeshFilter>();
        mf.mesh = shardMesh;

        MeshRenderer mr = shard.AddComponent<MeshRenderer>();
        mr.material = GetComponent<MeshRenderer>().material;

        // Collider + Rigidbody
        BoxCollider col = shard.AddComponent<BoxCollider>();
        Rigidbody rb = shard.AddComponent<Rigidbody>();

        // 힘 주기
        Vector3 randomDir = Random.onUnitSphere + Vector3.up * 0.3f;
        rb.AddForce(randomDir.normalized * explosionForce, ForceMode.Impulse);
    }
}
