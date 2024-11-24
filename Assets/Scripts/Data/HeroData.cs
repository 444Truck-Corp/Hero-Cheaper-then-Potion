using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Status
{
    public int STR; // 근력
    public int DEX; // 민첩
    public int INT; // 지능
    public int LUK; // 행운

    public Status(int _str, int _dex, int _int, int _luk)
    {
        STR = _str;
        DEX = _dex;
        INT = _int;
        LUK = _luk;
    }
    public Status SetStatus(int _str, int _dex, int _int, int _luk)
    {
        STR = _str;
        DEX = _dex;
        INT = _int;
        LUK = _luk;
        return this;
    }

    public static Status operator +(Status a, Status b)
    {
        return new Status(
            a.STR + b.STR,
            a.DEX + b.DEX,
            a.INT + b.INT,
            a.LUK + b.LUK
        );
    }
}

[Serializable]
public class HeroData
{
    private readonly List<string> names = new()
    {
            "에이다", "아델", "아가사", "아이다", "앨리쉬", "에이미", "알렉산드라", "앨리스", "앨리카", "앨리",
            "아만다", "안젤라", "안젤리카", "안나", "애니", "아리아", "아리엘", "아샤", "아우로라", "에이바",
            "바바라", "베키", "벨리타", "벨라", "벨", "베시", "비안나", "비앙카", "비비안", "칼리",
            "칼리아", "카밀라", "카라", "카르멘", "캐시", "캐서린", "세실", "셀레스틴", "셀리나", "샤샤",
            "샤비", "첼시", "쉐리", "클로에", "신디", "클라라", "클라우디아", "클레오", "코코", "코니",
            "쿠키", "크리스탈", "델라", "델핀", "데바", "도로시", "에코", "엘리스", "엘린", "엘리샤",
            "엘리자베스", "엘사", "엘리시아", "에밀리", "에스더", "에바", "이브", "플로리아", "젬마", "글로리아",
            "그레이스", "헬렌", "헬리아", "헤라", "헤스티아", "이리나", "아이리스", "이사도라", "이사벨", "이시스",
            "제인", "자스민", "제스퍼", "제니", "제니퍼", "제리", "제시카", "제시", "조안나", "졸리",
            "조이", "쥴리아", "쥴리엣", "쥴리아나", "카라", "카레나", "카시아", "케이트", "켈리", "키티",
            "키라", "라라", "라비나", "리나", "리디아", "릴리", "린다", "리사", "로티", "루시아",
            "루시", "루나", "매기", "맬리사", "마나", "마가레트", "메어리", "마틸다", "마야", "멜리나",
            "메리엘", "미란다", "미시", "미스티", "몰리", "모니카", "뮬란", "나미", "나오미", "니아",
            "니키", "니나", "올가", "올리비아", "파멜라", "페기", "레이나", "라비", "레이", "리사",
            "로빈", "로자", "로지", "록시", "샐리", "사라", "사샤", "스칼렛", "셀리나", "세레나",
            "사샤", "쉐리", "실키", "소니아", "소피", "스텔라", "써니", "슈가", "실비아", "트리샤",
            "트루디아", "바네사", "벨리카", "베라", "베로니카", "빅토리아", "바이올렛", "제키", "제나", "지나"
        };
    private readonly List<int> levelExpList = new() { 1, 2, 5, 6, 6, 8, 8, 10, 11, 12, 12 };

    public int id;
    public string name;
    public bool spriteType;
    public ClassData classData;
    public Status status;
    public int level;
    public int exp;
    public int spriteIdx;
    public int[] remainDay; // 훈련 후 남은 누적일. 방 4개가 상이

    public HeroData()
    {
        id = 0;
        name = "";
        classData = null;
        status = null;
        spriteType = false;
        level = 0;
        exp = 0;
        spriteIdx = 0;
        remainDay = new int[4];
    }

    public void Initialize(int characterCount)
    {
        id = characterCount;
        name = names[UnityEngine.Random.Range(0, names.Count)];
        classData = DataManager.Instance.GetData<ClassData>(nameof(ClassData), UnityEngine.Random.Range(0, 8));
        status = classData.BaseStat;
        spriteType = UnityEngine.Random.Range(0, 2) == 0;

        spriteIdx = classData.id * 2;
        spriteIdx = (spriteType) ? spriteIdx - 1 : spriteIdx - 2;
    }

    public void GetExp(int delta)
    {
        exp += delta;
        GameManager.Instance.OnGetExpEvent?.Invoke(this, delta);
        while (exp >= levelExpList[level] && level < 10)
        {
            exp -= levelExpList[level++];
            status += classData.IncStat;
            GameManager.Instance.OnHeroLevelUpEvent?.Invoke(this, level);
            GameManager.Instance.OnHeroStatUpEvent?.Invoke(this, classData.IncStat);
        }
    }

    public void Dead()
    {
        GameManager.Instance.OnHeroDeadEvent?.Invoke(this);
    }
}