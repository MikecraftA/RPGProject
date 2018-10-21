using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGWORLD
{
    public abstract class RPGWorld : MonoBehaviour
    {
        protected World world;

        public virtual void Start()
        {
            
        }

        public virtual void Update()
        {

        }
    }


    public abstract class World
    {
        
        private Loglevel lg = Loglevel.NONE;
        protected int seed;

        // No need for this for now
        //private Queue<Model> RenderQueue = new Queue<Model>();
        //private Queue<Model> DoneRenderQueue = new Queue<Model>();
        //private int MAXRENDER = 100;
        //private int BUSYTHRESH = 20;

        // Fields accessible from subclasses
        protected List<Model> Models = new List<Model>();

        public World(int seed)
        {
            this.seed = seed;
            Generate(seed);
        }
        
        // Leave to the users
        public abstract void Generate(int seed);

        public void Update()
        {
            
        }
        
        public bool AddModel(Model model)
        {
            /*
            if (RenderQueue.Count > MAXRENDER)
            {
                if(lg == Loglevel.ALL)
                {
                    Debug.LogWarning("Too busy rendering...");
                }
                return false;
            }
            */

            if (Models.Contains(model))
            {
                if (lg == Loglevel.ALL)
                {
                    Debug.LogWarning("Model already added");
                }
                return false;
            }

            Models.Add(model);
            model.Render();
            //RenderQueue.Enqueue(model);
            model.SetWorld(this);
            return true;
        }

        public bool DestroyModel(Model model)
        {
            if(lg == Loglevel.ALL && !Models.Contains(model))
            {
                Debug.LogWarning("Destroy fails because no model found");
            }
            bool r = Models.Remove(model);

            model.Destroy();

            return r;
        }

        protected float LoadingProgress = 0.0f;

        /*
         * For future external use (maybe GUI or logging)
         */
        public float GetProgress()
        {
            return LoadingProgress;
        }

        /*
        protected void SetMaxRenderSize(int size)
        {
            MAXRENDER = size;
        }
        */
        public void SetLogLevel(Loglevel l)
        {
            lg = l;
        }

        public Loglevel GerLogLevel()
        {
            return lg;
        }
    }   
    
    public abstract class Model
    {
        protected World world;
        protected GameObject ModelObj;
        protected bool isRendered = false;

        public Model()
        {

        }

        public void SetWorld(World w)
        {
            world = w;
        }

        /*
         * Not forced rendering
         */
        public virtual void Render()
        {
            if (!isRendered)
            {
                ModelObj = new GameObject();
                MeshFilter mf = ModelObj.AddComponent<MeshFilter>();
                MeshRenderer mr = ModelObj.AddComponent<MeshRenderer>();
                Update();
            }
        }

        /*
         * Forced rendering
         */
        public virtual void Update()
        {
            isRendered = true;
        }

        protected void UpdateMesh(Mesh mesh)
        {
            if (ModelObj == null && world.GerLogLevel() == Loglevel.ALL)
            {
                Debug.LogWarning("Model is either destroyed or have not rendered");
            } else
            {
                MeshFilter mf = ModelObj.GetComponent<MeshFilter>();
                MeshRenderer mr = ModelObj.GetComponent<MeshRenderer>();
                if (!mf || !mr && world.GerLogLevel() == Loglevel.ALL)
                {
                    Debug.LogWarning("Model is either destroyed or have not rendered");
                }
                else
                {
                    mf.mesh = mesh;
                }
            }
        }

        public virtual void Hide()
        {
            if (ModelObj != null)
            {
                ModelObj.SetActive(false);
            }
            else if (world.GerLogLevel() == Loglevel.ALL)
            {
                Debug.LogWarning("Model is either destroyed or have not rendered");
            }
        }

        public virtual void Show()
        {
            if (ModelObj != null)
            {
                ModelObj.SetActive(true);
            }
            else if (world.GerLogLevel() == Loglevel.ALL)
            {
                Debug.LogWarning("Model is either destroyed or have not rendered");
            }
        }

        public virtual void Destroy()
        {
            if (ModelObj != null)
            {
                Object.Destroy(ModelObj);
            } else if(world.GerLogLevel() == Loglevel.ALL)
            {
                Debug.LogWarning("Model already destoryed"); 
            }
        }
    }

    public enum Loglevel
    {
        ALL, NONE
    }
}

