using UnityEditor;

namespace Necrosoft.Editor
{
    [InitializeOnLoad]
    public static class ExecutionOrderEditor
    {
        static ExecutionOrderEditor()
        {
            MonoScript[] scripts = MonoImporter.GetAllRuntimeMonoScripts();
            for (int i = 0; i < scripts.Length; ++i) {
                if (scripts[i].GetClass() == null) continue;

                ExecutionOrder[] attributes = System.Attribute.GetCustomAttributes(scripts[i].GetClass(), typeof(ExecutionOrder)) as ExecutionOrder[];
                for (int n = 0; n < attributes.Length; ++n) {
                    if (MonoImporter.GetExecutionOrder(scripts[i]) == attributes[n].executionOrder) continue;
                    MonoImporter.SetExecutionOrder(scripts[i], attributes[n].executionOrder);
                }
            }
        }
    }
}