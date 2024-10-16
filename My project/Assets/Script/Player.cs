using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�Q�[���̃v���C���[�܂���AI�����p�ł���|�C���g���v�Z
public abstract class Player : MonoBehaviour
{// Stone�N���X��Color�v���p�e�B�𒊏ۃv���p�e�B�Ƃ��Ē�`
    public abstract Stone.Color MyColor { get; }

    // �΂��I������Ă��邩�ǂ������`�F�b�N���鉼�z���\�b�h�B
    // ������Ԃł́A�I������Ă��Ȃ����Ƃ��������߂ɏ��false��Ԃ��B
    public virtual bool TryGetSelected(out int x, out int z)
    {
        x = 0;
        z = 0;
        return false;
    }

    // �v���C���[���΂�u�����Ƃ��ł���ʒu�ƁA���̈ʒu�œ�����|�C���g�����v�Z���郁�\�b�h�B
    // ���p�\�ȃ|�C���g���i�[����Dictionary��Ԃ��B
    public Dictionary<Tuple<int, int>, int> CalcAvailablePoints()
    {
        // �Q�[���̃C���X�^���X���擾
        var game = Game.Instance;

        // ���݂̐΂̔z�u���擾
        var stones = game.Stones;

        // ���p�\�ȃ|�C���g���i�[����Dictionary��������
        var availablePoints = new Dictionary<Tuple<int, int>, int>();

        // �Q�[���Ղ̑S�Ă̍��W�𑖍�
        for (var z = 0; z < Game.ZNum; z++)
        {
            for (var x = 0; x < Game.XNum; x++)
            {
                // ���݂̍��W�ɐ΂��u����Ă��Ȃ��ꍇ
                if (stones[z][x].CurrentState == Stone.State.None)
                {                                                                           //�f�[�^�{���Ƀo�O����
                    // ���݂̐F�̐΂�u�����ꍇ�ɂЂ�����Ԃ���΂̐����v�Z
                    var reverseCount = game.CalcTotalReverseCount(MyColor, x, z);

                    // �Ђ�����Ԃ���΂�����ꍇ
                    if (reverseCount > 0)
                    {
                        // ���p�\�ȃ|�C���g�Ƃ���Dictionary�ɒǉ�
                        availablePoints[Tuple.Create(x, z)] = reverseCount;
                    }
                }
            }
        }

        // ���p�\�ȃ|�C���g��Dictionary��Ԃ�
        return availablePoints;
    }


    // �v���C���[���΂�u����ꏊ�����邩�ǂ����𔻒肷�郁�\�b�h
    public bool CanPut()
    {
        // ���p�\�ȃ|�C���g���v�Z���A���̐���0���傫�����ǂ������`�F�b�N
        return CalcAvailablePoints().Count > 0;
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
