using System;
using System.Collections.Generic;
using System.Text;

internal interface IWorkable
{
    /// <summary>
    /// 생산량을 반환하는 Method. 생산량은 
    /// </summary>
    /// <param name="isHalf">생산량 반토막 적용 여부</param>
    /// <returns>시간당 총 생산량 반환</returns>
    public int Work(bool isHalf);

    /// <summary>
    /// 인간 노동자 추가
    /// </summary>
    /// <returns>성공 여부 출력</returns>
    public bool AssignAnthroposWorker();

    /// <summary>
    /// 인간 노동자 감소
    /// </summary>
    /// <returns>성공 여부 출력</returns>
    public bool FireAnthroposWorker();

    /// <summary>
    /// 인형 노동자 추가
    /// </summary>
    /// <returns>성공 여부 출력</returns>
    public bool AssignGynaikoeidesWorker();

    /// <summary>
    /// 인형 노동자 감소
    /// </summary>
    /// <returns>성공 여부 출력</returns>
    public bool FireGynaikoeidesWorker();
}
