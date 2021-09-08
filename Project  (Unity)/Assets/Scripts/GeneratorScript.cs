using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics;
using UnityEngine;
using MathNet.Numerics.Distributions;
using Random = UnityEngine.Random;

public class GeneratorScript : MonoBehaviour
{
    public GameObject[] availableRooms;
    public List<GameObject> currentRooms;
    private float screenWidthInPoints;
    public GameObject[] availableBonus;
    public GameObject[] availableEnemies;
    public List<GameObject> objects;
    public float objectsMinY = -1.4f;
    public float objectsMaxY = 1.4f;
    public float objectsMinRotation = -45.0f;
    public float objectsMaxRotation = 45.0f;
    public Poisson p = new Poisson(8);
    public Geometric geo = new Geometric(0.1327);
    public Hypergeometric hyper = new Hypergeometric(10, 4, 6);
    public DiscreteUniform disc = new DiscreteUniform(5,15);

    // Use this for initialization
    private void Start(){
        
    float height = 2.0f * Camera.main.orthographicSize;
        screenWidthInPoints = height * Camera.main.aspect;
        StartCoroutine(GeneratorCheck());


        // Update is called once per frame
        void Update()
        {
        }

        void AddRoom(float farhtestRoomEndX)
        {
            
            //1
            int randomRoomIndex = hyper.Sample();
            while (randomRoomIndex >= availableRooms.Length)
            {
                randomRoomIndex = hyper.Sample();
            }
            Console.Write(randomRoomIndex);

            //2
            GameObject room = (GameObject) Instantiate(availableRooms[randomRoomIndex]);

            //3
            float roomWidth = room.transform.Find("floor").localScale.x;

            //4
            float roomCenter = farhtestRoomEndX + roomWidth * 0.5f;

            //5
            room.transform.position = new Vector3(roomCenter, 0, 0);

            //6
            currentRooms.Add(room);
        }

        void GenerateRoomIfRequired()
        {
            //1
            List<GameObject> roomsToRemove = new List<GameObject>();
            //2
            bool addRooms = true;
            //3
            float playerX = transform.position.x;
            //4
            float removeRoomX = playerX - screenWidthInPoints;
            //5
            float addRoomX = playerX + screenWidthInPoints;
            //6
            float farthestRoomEndX = 0;
            foreach (var room in currentRooms)
            {
                //7
                float roomWidth = room.transform.Find("floor").localScale.x;
                float roomStartX = room.transform.position.x - (roomWidth * 0.5f);
                float roomEndX = roomStartX + roomWidth;
                //8
                if (roomStartX > addRoomX)
                {
                    addRooms = false;
                }

                //9
                if (roomEndX < removeRoomX)
                {
                    roomsToRemove.Add(room);
                }

                //10
                farthestRoomEndX = Mathf.Max(farthestRoomEndX, roomEndX);
            }

            //11
            foreach (var room in roomsToRemove)
            {
                currentRooms.Remove(room);
                Destroy(room);
            }

            //12
            if (addRooms)
            {
                AddRoom(farthestRoomEndX);
            }
        }

        IEnumerator GeneratorCheck()
        {
            while (true)
            {
                GenerateRoomIfRequired();
                GenerateObjectsIfRequired();
                yield return new WaitForSeconds(0.25f);
            }
        }

        void AddBonus(float lastObjectX)
        {
            //1
            int randomIndex = p.Sample();
            int randomPos = disc.Sample();
            while (randomIndex >= availableBonus.Length)
            {
                randomIndex = p.Sample();
            }
            //2
            GameObject obj = (GameObject)Instantiate(availableBonus[randomIndex]);
            //3
            float objectPositionX = lastObjectX + randomPos;
            float randomY = Random.Range(objectsMinY, objectsMaxY);
            obj.transform.position = new Vector3(objectPositionX, randomY, 0);
            //4
            float rotation = Random.Range(objectsMinRotation, objectsMaxRotation);
            obj.transform.rotation = Quaternion.Euler(Vector3.forward * rotation);
            //5
            objects.Add(obj);
        }

        void AddEnemies(float lastObjectX)
        {
            //1
            int randomIndex = geo.Sample();
            int randomPos = disc.Sample();
            while (randomIndex >= availableEnemies.Length)
            {
                Console.Write(randomIndex);
                randomIndex = geo.Sample();
            }
            Console.Write(randomIndex);
            //2
            GameObject obj = (GameObject)Instantiate(availableEnemies[randomIndex]);
            //3
            float objectPositionX = lastObjectX + randomPos;
            float randomY = Random.Range(objectsMinY, objectsMaxY);
            obj.transform.position = new Vector3(objectPositionX, randomY, 0);
            //4
            float rotation = Random.Range(objectsMinRotation, objectsMaxRotation);
            obj.transform.rotation = Quaternion.Euler(Vector3.forward * rotation);
            //5
            objects.Add(obj);
        }

        void GenerateObjectsIfRequired()
        {
            //1
            float playerX = transform.position.x;
            float removeObjectsX = playerX - screenWidthInPoints;
            float addObjectX = playerX + screenWidthInPoints;
            float farthestObjectX = 0;
            //2
            List<GameObject> objectsToRemove = new List<GameObject>();
            foreach (var obj in objects)
            {
                if (obj != null)
                {
                    //3
                    float objX = obj.transform.position.x;
                    //4
                    farthestObjectX = Mathf.Max(farthestObjectX, objX);
                    //5
                    if (objX < removeObjectsX)
                    {
                        objectsToRemove.Add(obj);
                    }
                }
            }

            //6
            foreach (var obj in objectsToRemove)
            {
                objects.Remove(obj);
                Destroy(obj);
            }

            //7
            if (farthestObjectX < addObjectX)
            {
                if(Random.Range(1,10).IsEven())
                {
                    AddBonus(farthestObjectX);
                }
                else
                {
                    AddEnemies(farthestObjectX);
                }
            }
        }

    }
}