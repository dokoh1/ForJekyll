using UnityEngine;

public interface IAttackable
{
    void OnHitSuccess();

    void OnHitSuccess(UnitEnum.UnitType unitType);
}
