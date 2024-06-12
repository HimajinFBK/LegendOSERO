using System;
using System.Collections.Generic;

using System.Linq;

public class EnemeyPlayer : Player
{  //‘Îí‘ŠŽè(CPU)
    //ˆê”ÔŽæ‚ê‚éêŠ‚ðŽæ‚éŽd—l
    public override Stone.Color MyColor { get { return Stone.Color.White; } }

    private Random _random = new Random();

    public override bool TryGetSelected(out int x, out int z)
    {
        var availablePoints = CalcAvailablePoints();
        var maxCount= availablePoints.Values.Max();
        var list=availablePoints.Where(p=>p.Value==maxCount).Select(p=>p.Key).ToList();
        if(list.Count>0)
        {
            var point = list[_random.Next(list.Count)];
            x = point.Item1;
            z = point.Item2;

            return true;
        }
        else
        {
            throw new Exception("Invalid State Enemy Cannot Put Stone.");
        }
    }
}
