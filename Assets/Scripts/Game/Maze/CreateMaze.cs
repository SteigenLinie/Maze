using UnityEngine;

public class CreateMaze : MonoBehaviour
{
	/// ���������� ������ � ������� ��������� (�������� �����)
	public byte size = 5; // ������������ �������� size � 47
	/// ���������� ������ �� ������ ������
	public byte widthCount = 2;
	/// ���������� ������ �� ������ ������
	public byte heightCount = 3;

	/// ������ ��� �������� ���������
	int[,] m;

	void Awake()
	{
		// ����������� ��������� ������� "m":  1 - �����, 0 - �����, 2 - ����� ������ ��������� � ������
		// ��������� ���� ������ ���� ��������������
		m = new int[size + 1, size + 2];

		// �������� ���������� �������� ��� ���������� ��������� �����
		Random.seed = Random.Range(int.MinValue, int.MaxValue); // [ 0; 0x7FFFFFFF ]

		GenerateMaze();

		Mesh mesh = BuildModel();
		gameObject.GetComponent<MeshFilter>().mesh = mesh;
		gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;

		int ps = (size + 1) / 2; // primary size
								 // ���������� ������ ��������� ������� ���������
		GameObject.FindWithTag("Player").transform.position = new Vector3(.5f + 2 * Random.Range(0, ps), .9f, .5f);
	}

	/// ���������� ��������� ������� true ��� false
	bool BoolRandom()
	{
		return Random.Range(0, 2) == 0;
	}

	/// ������, �� ������������� �� � ������ �� ��������, ��� ������ �������
	const int empty = 1;
	/// ������� ����� ��������
	const int boundary = 0;
	/// ��������� ��������� � ������� ��������� �������
	/*
	 	 ___ ___ ___        ,� � � � � �,
		|  7   8|  9|       �_�_�_�_�_���
		|    ___|   |       |7|_|8|�|9|��
		|  4|  5   6|  ��>  |_|�|�|�|_|��
		|   |       |       |4|�|5|_|6|��
		|  1   2|  3|       |_|_|_|�|_|��
		|___ ___|___|       |1|_|2|�|3|��
		
		����������� ��������� ������� "m":
			1 - ������ �� ��������� ������ �� ������ ���������
			0 - ������� ����� �������� ("|��|")
			|1|, |2|, |3|, ... - ����� ���������
	*/
	void GenerateMaze()
	{
		for (byte y = 0; y < size; y++)
			for (byte x = 0; x <= size; x++)
				m[x, y] = empty; // ��� �������� ���������� �����, � ������ �� ��������� ������ �� ������ ���������

		int setCount = -1;
		byte size_m_2 = (byte)(size - 2);
		for (byte y = 0; y < size; y += 2)
		{
			// ������������ �������, �� �������� � ���������, ���� ���������� ���������
			for (byte x = 0; x < size; x += 2)
				if (m[x, y] == empty)
				{
					m[x, y] = setCount;
					setCount--;
				}
			// �������� ������ ������
			for (byte x = 0; x < size_m_2; x += 2)
			{
				// ���� ������� ������ � ������ ������ ����������� ������ ���������,
				if (m[x, y] == m[x + 2, y] || BoolRandom())
				{
					m[x + 1, y] = boundary; // �� ������� ������� ����� ���� (��� �������������� ������������)
					m[x + 1, y + 1] = boundary;
				}
				else                    // ���� ������� �� �����������, �� ��������� ��� ���������
					m[x + 2, y] = m[x, y];  // � ������� ��������� ������� ������ � ������ ������
			}
			// �������� ������� ������
			for (byte x = 0; x < size_m_2; x += 2)
				if (m[x, y] == m[x + 2, y])
					if (BoolRandom())
					{
						m[x, y + 1] = boundary;
						m[x + 1, y + 1] = boundary;
					}
					else
					{
						m[x + 2, y + 1] = boundary;
						m[x + 3, y + 1] = boundary;
					}
			for (byte x = 0; x < size; x += 2)
			{
				// ����������� ������� ������ �� ����
				m[x, y + 2] = m[x, y];
				// �������� ����� � ������ �������� �� �� ���������
				if (m[x, y + 1] == boundary) m[x, y + 2] = empty;
			}
		}
		int size_m_1 = (byte)(size - 1);
		// ������������ ��������� ������ ���������
		for (byte x = 0; x < size_m_2; x += 2)
			// ���� ������� ������ � ������ ������ ����� ������ ��������, ��
			if (m[x, size_m_1] != m[x + 2, size_m_1])
				m[x + 1, size_m_1] = empty; // ������� ������ �������

		for (byte x = 2; x < size; x += 2)
			for (byte y = 2; y < size; y += 2)
				if (m[x - 1, y] == boundary || m[x, y - 1] == boundary)
					m[x - 1, y - 1] = boundary; // ����������� ����� � ������� � ����� ����� L
												// ������� ��������� ������
		for (byte x = 0; x <= size; x++) m[x, size] = empty;
		// ��� ������ ������� �������
		for (byte y = 0; y < size; y += 2)
			for (byte x = 0; x < size; x += 2) m[x, y] = empty;
	}

	int getMeshesNumber()
	{
		int wallCount = 0;   // ���������� ����-������
		for (byte y = 0; y < size; y++)
			for (byte x = 0; x < size; x++)
				if (m[x, y] == boundary)
					wallCount++;
		int emptyCount = size * size - wallCount;
		return 4 * emptyCount + 2 + size;  //6*emptyCount - 2*(emptyCount-1)
	}

	/// �������� ������ ���������
	Mesh BuildModel()
	{
		Mesh wallMesh = Wall(Vector2.zero, new Vector2(.5f, 0), Vector2.up); // �������� ��������
		Mesh groundMesh = Quad(Vector3.up, Vector3.right, Vector3.forward,
							   new Vector2(.5f, 0), Vector2.right, new Vector2(.5f, .5f));
		Mesh roofMesh = Quad(Vector3.down, Vector3.forward, Vector3.right,
							 new Vector2(.5f, .5f), new Vector2(1, .5f), new Vector2(.5f, 1));
		Vector3 scale = new Vector3(widthCount, 1, widthCount);
		Quaternion bottom = Quaternion.identity;
		Quaternion left = Quaternion.Euler(0, 90, 0);
		Quaternion top = Quaternion.Euler(0, 180, 0);
		Quaternion right = Quaternion.Euler(0, -90, 0);
		CombineInstance[] meshes = new CombineInstance[getMeshesNumber()];
		int faceOffset = 0, sX, sZ, lastRow = size * widthCount;
		// ������ ������ ��������� �� ������ ��������� ������� m
		for (int z = 0; z < size; z++)
		{
			for (int x = 0; x < size; x++)
			{
				sX = x * widthCount; // scale X
				sZ = z * widthCount; // scale Y
				if (m[x, z] == boundary)
				{
					if (z > 0 && m[x, z - 1] > boundary)
					{       // ����� �� ������� ������
						meshes[faceOffset].mesh = wallMesh;
						meshes[faceOffset].transform = Matrix4x4.TRS(new Vector3(sX, 0, sZ), bottom, Vector3.one);
						faceOffset++;
					}
					if (x > 0 && m[x - 1, z] > boundary)
					{       // ����� �� ������� ������
						meshes[faceOffset].mesh = wallMesh;
						meshes[faceOffset].transform = Matrix4x4.TRS(
							new Vector3(sX, 0, sZ + widthCount), left, Vector3.one);
						faceOffset++;
					}
					if (z < size && m[x, z + 1] > boundary)
					{   // ������ �� ������� ������
						meshes[faceOffset].mesh = wallMesh;
						meshes[faceOffset].transform = Matrix4x4.TRS(
							new Vector3(sX + widthCount, 0, sZ + widthCount), top, Vector3.one);
						faceOffset++;
					}
					if (x < size && m[x + 1, z] > boundary)
					{   // ������ �� ������� ������
						meshes[faceOffset].mesh = wallMesh;
						meshes[faceOffset].transform = Matrix4x4.TRS(
							new Vector3(sX + widthCount, 0, sZ), right, Vector3.one);
						faceOffset++;
					}
				}
				else
				{
					meshes[faceOffset].mesh = groundMesh;   // ���
					meshes[faceOffset].transform = Matrix4x4.TRS(new Vector3(sX, 0, sZ), bottom, scale);
					faceOffset++;
					meshes[faceOffset].mesh = roofMesh;     // �����
					meshes[faceOffset].transform = Matrix4x4.TRS(new Vector3(sX, heightCount, sZ), bottom, scale);
					faceOffset++;
				}
			}
			// �������� ������
			sZ = z * widthCount;    // scale Z
			sX = sZ + widthCount;   // scale X
			meshes[faceOffset].mesh = wallMesh; // "������" ������
			meshes[faceOffset].transform = Matrix4x4.TRS(new Vector3(sX, 0, 0), top, Vector3.one);
			faceOffset++;
			meshes[faceOffset].mesh = wallMesh; // "�����" ������
			meshes[faceOffset].transform = Matrix4x4.TRS(new Vector3(0, 0, sZ), right, Vector3.one);
			faceOffset++;
			meshes[faceOffset].mesh = wallMesh; // "�������" ������
			meshes[faceOffset].transform = Matrix4x4.TRS(new Vector3(sZ, 0, lastRow), bottom, Vector3.one);
			faceOffset++;
			meshes[faceOffset].mesh = wallMesh; // "������" ������
			meshes[faceOffset].transform = Matrix4x4.TRS(new Vector3(lastRow, 0, sX), left, Vector3.one);
			faceOffset++;
		}

		Mesh mesh = new Mesh();
		mesh.CombineMeshes(meshes, true, true);
		mesh.Optimize();
		return mesh;
	}

	/// <summary>�������� ������ �����</summary>
	/// <param name="uv0">���������� ���������� (0; 0)</param>
	/// <param name="uvRightBottom">���������� ���������� (1; 0)</param>
	/// <param name="uvLeftTop">���������� ���������� (0; 1)</param>
	Mesh Wall(Vector2 uv0, Vector2 uvRightBottom, Vector2 uvLeftTop) // ��������� - ���������� ����������
	{
		Vector2 uvX = (uvRightBottom - uv0) / widthCount;
		Vector2 uvY = (uvLeftTop - uv0) / heightCount;
		int yOffset = widthCount + 1; // ���������� ����� � ������ �� ���� ������ ��� ������
		int sq = yOffset * (heightCount + 1);
		Vector3[] vertices = new Vector3[sq];
		int[] triangles = new int[6 * widthCount * heightCount];
		Vector2[] uv = new Vector2[sq];
		int lastY = sq - widthCount - 1; // == (widthCount+1)*heightCount
		int p2, offset, trianglesOffset = 0;
		// ������ ������
		for (int y = 0; y < heightCount; y++)
		{
			for (int x = 0; x < widthCount; x++)
			{
				offset = x + yOffset * y;
				vertices[offset] = new Vector3(x, y, 0); // pos + x*axisX + y*Vector3.up
				uv[offset] = uv0 + x * uvX + y * uvY;
				p2 = x + 1 + yOffset * (y + 1);
				triangles[6 * trianglesOffset] = offset;
				triangles[6 * trianglesOffset + 1] = p2 - 1;
				triangles[6 * trianglesOffset + 2] = p2;
				triangles[6 * trianglesOffset + 3] = offset;
				triangles[6 * trianglesOffset + 4] = p2;
				triangles[6 * trianglesOffset + 5] = p2 - yOffset;
				trianglesOffset++;
			}
			vertices[widthCount + yOffset * y] = new Vector3(widthCount, y, 0); //pos + widthCount*axisX + y*Vector3.up;
			uv[widthCount + yOffset * y] = uv0 + widthCount * uvX + y * uvY;
		}
		for (int x = 0; x < widthCount; x++)
		{
			vertices[x + lastY] = new Vector3(x, heightCount, 0); //pos + y*axisX + heightCount*Vector3.up;
			uv[x + lastY] = uv0 + x * uvX + heightCount * uvY;
		}
		vertices[sq - 1] = new Vector3(widthCount, heightCount, 0); //pos + widthCount*axisX + heightCount*Vector3.up;
		uv[sq - 1] = uv0 + widthCount * uvX + heightCount * uvY;
		Mesh result = new Mesh();
		result.vertices = vertices;
		result.triangles = triangles;
		result.uv = uv;
		result.RecalculateNormals();
		return result;
	}

	/// <summary>�������� ��������������� ��������</summary>
	/// <param name="normal">������ �������</param>
	/// <param name="X">����� �� ��� X</param>
	/// <param name="Z">����� �� ��� Z</param>
	/// <param name="uv0">���������� ���������� (0; 0)</param>
	/// <param name="uvRightBottom">���������� ���������� (1; 0)</param>
	/// <param name="uvLeftTop">���������� ���������� (0; 1)</param>
	Mesh Quad(Vector3 normal, Vector3 X, Vector3 Z, Vector2 uv0, Vector2 uvRightBottom, Vector2 uvLeftTop)
	{
		return new Mesh
		{
			vertices = new[] { Vector3.zero, Z, new Vector3(1, 0, 1), X },
			normals = new[] { normal, normal, normal, normal },
			uv = new[] { uv0, uvLeftTop, new Vector2(uvRightBottom.x, uvLeftTop.y), uvRightBottom },
			triangles = new[] { 0, 1, 2, 0, 2, 3 }
		};
	}
}