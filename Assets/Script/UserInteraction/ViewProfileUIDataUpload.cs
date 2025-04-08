using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ViewProfileUIDataUpload : MonoBehaviour
{


    public TextMeshProUGUI[] SenderDetailsText;

    // Start is called before the first frame update

    public void SetdetailProfile(string pinNum)
    {
        UserData userdata = DatabaseManager.Instance.getUserData(pinNum);

        SenderDetailsText[0].text = userdata.name;
        SenderDetailsText[1].text = userdata.job;
        SenderDetailsText[2].text = userdata.introduction_text;
        SenderDetailsText[3].text = userdata.introduction_1;
        SenderDetailsText[4].text = userdata.introduction_2;
        SenderDetailsText[5].text = userdata.introduction_3;
        //SenderDetailsText[6].text = userdata.introduction_4;
        //SenderDetailsText[7].text = userdata.introduction_5;
        SenderDetailsText[6].text = userdata.interest_1;
        SenderDetailsText[7].text = userdata.interest_2;
        SenderDetailsText[8].text = userdata.interest_3;
        //SenderDetailsText[11].text = userdata.interest_4;
        //SenderDetailsText[12].text = userdata.interest_5;
        SenderDetailsText[9].text = userdata.url;

    }
}
