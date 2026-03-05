using LitMotion;
using System;
using System.Collections.Generic;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Work.Enemies.Code;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "HitMaterialChange", story: "[Self] set material change from hit", category: "Action", id: "5ff653ea008dbb7e66c38549114f8749")]
public partial class HitMaterialChangeAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Self;

    private List<Material> _enemyMat = new List<Material>();

    protected override Status OnStart()
    {
        Renderer[] renderers = Self.Value.GetModule<EnemyAnimatorModule>().Renderers;

       

        foreach (var r in renderers)
        {
            if (r == null || r.material == null)
                return Status.Failure;
            _enemyMat.Add(r.material);
        }

        foreach (var mat in _enemyMat)
        {
            LMotion.Create(0f, 1f, 0.12f)
            .WithEase(Ease.OutExpo)
            .WithOnComplete(() => LMotion.Create(1f, 0f, 0.07f)
            .WithEase(Ease.InBack)
            .Bind(x => mat.SetFloat("_Value", x)))
            .Bind(x => mat.SetFloat("_Value", x));
        }

        
        
        return Status.Success;
    }

}

