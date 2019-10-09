using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IImitationStrategy
{
    float Execute(Boxer me, Boxer teacher, Reward reward);
}
