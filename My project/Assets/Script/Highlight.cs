
using UnityEngine;

public class Highlight : MonoBehaviour
{

    // �ʏ펞�̃I�u�W�F�N�g�̐F
    [SerializeField]
    private Color normalColor;

    // �}�E�X�I�[�o�[���̃I�u�W�F�N�g�̐F
    [SerializeField]
    private Color mouseOverColor;

    // �I�u�W�F�N�g�̃}�e���A������ێ�
    private Material material;

    // �X�N���v�g�����߂Ď��s���ꂽ�Ƃ��ɌĂ΂�郁�\�b�h
    private void Start()
    {
        // ���̃I�u�W�F�N�g��MeshRenderer����}�e���A�����擾���A�ێ�
        material = GetComponent<MeshRenderer>().material;

        // �I�u�W�F�N�g�̐F��ʏ펞�̐F�ɐݒ�
        material.color = normalColor;
    }

    // �}�E�X���I�u�W�F�N�g��ɏ�����Ƃ��ɌĂ΂�郁�\�b�h
    private void OnMouseEnter()
    {
        // �}�E�X�I�[�o�[���̐F�ɕύX
        material.color = mouseOverColor;
    }

    // �}�E�X���I�u�W�F�N�g�ォ�痣�ꂽ�Ƃ��ɌĂ΂�郁�\�b�h
    private void OnMouseExit()
    {
        // �ʏ펞�̐F�ɖ߂�
        material.color = normalColor;
    }

    // �I�u�W�F�N�g���j�������Ƃ��ɌĂ΂�郁�\�b�h
    private void OnDestroy()
    {
        // �}�e���A����j�����A������������i�C���X�^���X���Ƃ̃}�e���A�����g�p���Ă���ꍇ�j
        Destroy(material);
    }

}
