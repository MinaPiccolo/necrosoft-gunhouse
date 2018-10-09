using Necrosoft.Types;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Necrosoft
{
    public class Pool
    {
        static StackFixed<GameObject>[] pools = new StackFixed<GameObject>[20]; /* Set this to something that works for you */
        static int count;

        #region Create

        public static void CreateImmediate(GameObject prefab, int size) { CreateImmediate(prefab, size, null); }
        public static void CreateImmediate(GameObject prefab, int size, Transform parent) { CreateImmediate(prefab.GetComponent<Poolable>(), size, parent); }

        public static void CreateImmediate(Poolable prefab, int size) { CreateImmediate(prefab, size, null); }
        public static void CreateImmediate(Poolable prefab, int size, Transform parent)
        {
            Assert.IsNotNull(prefab, "Cannot Pool.Create(). Poolable.cs must be added to GameObject.");

            prefab.id = count;
            count++;
            pools[prefab.id] = new StackFixed<GameObject>(size);

            Transform root = new GameObject("Pool" + prefab.name).transform;
            if (parent != null) root.parent = parent;

            for (int i = 0; i < size; ++i) {
                Poolable item = Object.Instantiate(prefab);
                item.gameObject.SetActive(false);
                item.name = string.Format("{0}{1}", prefab.name, item.GetInstanceID());
                item.id = prefab.id;
                item.transform.parent = root;
                pools[prefab.id].Push(item.gameObject);

                #if UNITY_EDITOR
                item.poolSize = size;
                #endif
            }
        }

        public static void Create(MonoBehaviour script, GameObject prefab, int size) { Create(script, prefab, size, null); }
        public static void Create(MonoBehaviour script, GameObject prefab, int size, Transform parent) { Create(script, prefab.GetComponent<Poolable>(), size, parent); }
        public static void Create(MonoBehaviour script, Poolable prefab, int size) { Create(script, prefab, size, null); }
        public static void Create(MonoBehaviour script, Poolable prefab, int size, Transform parent) { script.StartCoroutine(Create(prefab, size, parent)); }

        static IEnumerator Create(Poolable prefab, int size, Transform parent)
        {
            Assert.IsNotNull(prefab, "Cannot Pool.Create(). Poolable.cs must be added to GameObject.");

            prefab.id = count;
            count++;

            pools[prefab.id] = new StackFixed<GameObject>(size);

            Transform root = new GameObject("Pool" + prefab.name).transform;
            if (parent != null) root.parent = parent;

            for (int i = 0; i < size; ++i) {
                Poolable item = Object.Instantiate(prefab);
                item.gameObject.SetActive(false);
                item.name = string.Format("{0}{1}", prefab.name, item.GetInstanceID());
                item.id = prefab.id;
                item.transform.parent = root;
                pools[prefab.id].Push(item.gameObject);

                #if UNITY_EDITOR
                item.poolSize = size;
                #endif

                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// Creates a fixed size pool from GameObjects already existing in the Scene.
        /// </summary>
        /// <param name="items">Items to Pool.</param>
        public static void CreateFrom(Poolable[] items)
        {
            Assert.IsTrue(count < pools.Length, "Exhausted StackFixed capacity.");

            int size = items.Length;
            pools[count] = new StackFixed<GameObject>(size);

            for (int i = 0; i < items.Length; ++i) {
                items[i].gameObject.SetActive(false);
                items[i].id = count;

                #if UNITY_EDITOR
                items[i].poolSize = size;
                #endif

                pools[count].Push(items[i].gameObject);
            }

            count++;
        }

        #endregion

        #region Request

        public static GameObject Request(GameObject prefab)
        {
            Poolable prefabPool = prefab.GetComponent<Poolable>();

            Assert.IsNotNull(prefab, "Cannot Pool.Request(). Poolable.cs must be added to GameObject.");

            GameObject gameObject = pools[prefabPool.id].Pop();

            Assert.IsNotNull(gameObject, "Cannot Pool.Request(). Pool exhausted.");

            gameObject.SetActive(true);
            return gameObject;
        }

        /// <summary>
        /// Requests any pool to return component.
        /// </summary>
        /// <returns>Pool with component.</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T RequestByComponent<T>() where T : Component
        {
            for (int i = 0; i < pools.Length; ++i) {
                if (pools[i].Size == 0) continue;
                if ((pools[i].Peek().GetComponent<T>()) == null) continue;

                T component = pools[i].Pop().GetComponent<T>();
                component.gameObject.SetActive(true);

                return component;
            }

            return null;
        }

        #endregion

        #region Return

        public static void Return<T>(T item) where T : Component { Return(item.GetComponent<Poolable>()); }
        public static void Return(GameObject gameObject) { Return(gameObject.GetComponent<Poolable>()); }
        public static void Return(Poolable poolable)
        {
            Assert.IsNotNull(poolable, "Cannot Pool.Return(). Poolable.cs must be added to GameObject");

            poolable.gameObject.SetActive(false);
            pools[poolable.id].Push(poolable.gameObject);
        }

        public static void ReturnAll<T>() where T : Component
        {
            T[] t = Object.FindObjectsOfType<T>(); 
            foreach (T item in t) {
                Return(item.GetComponent<Poolable>());
            }
        }

        #endregion

        #if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        static void OnScriptsReloaded()
        {
            Poolable[] items = Resources.FindObjectsOfTypeAll<Poolable>();
            for (int i = 0; i < items.Length; ++i) {
                if (items[i].gameObject.activeSelf) continue;

                if (pools[items[i].id] == null) {
                    pools[items[i].id] = new StackFixed<GameObject>(items[i].poolSize);
                }

                pools[items[i].id].Push(items[i].gameObject);
            }
        }
        #endif
    }
}