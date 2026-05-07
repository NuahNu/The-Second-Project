

#region ICVSData
/*
▶ 작성자 류연우

CGSSLoader에서 읽어온 데이터를 파싱할 수 있는 객체를 위한 인터페이스

    static readonly string NAME = "AbilityData";
처럼 이름을 저장하는 필드가 꼭 있는데 이거를 여기서 강제한다?



*/
#endregion

public interface ICSVData
{
    static readonly string NAME;

    public string ParseData(string data);

    public int ID { get; }
}
