using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class XMLChangerManager : MonoBehaviour
{
    private const string SuccessMessage = "Готово";
    private const string ErrorMessage = "Ошибка";

    public float HiddingSpeed = 0.2f;

    public InputField XMLsName;
    public InputField Angle;
    public Text Message;

    public string[] xmlsNames;

    private void Start()
    {
        var path = Directory.GetCurrentDirectory();
        xmlsNames = Directory.GetFiles(path, "*.xml");
    }

    public void OnClickAcceptButton()
    {
        bool success = false;

        if (xmlsNames != null && xmlsNames.Length > 0)
        {
            try
            {
                string name = XMLsName.text.Split('.')[0];
                if (!string.IsNullOrEmpty(name) && int.TryParse(Angle.text, out int angle))
                {
                    string xmlFilename = xmlsNames.FirstOrDefault(h => Utilities.GetNameFromPath(h).Split('.')[0] == name);
                    if (!string.IsNullOrEmpty(xmlFilename))
                    {
                        XDocument doc = null;
                        using (StreamReader reader = new StreamReader(xmlFilename))
                        {
                            doc = XDocument.Load(reader);
                            var elements = doc.Element("elements").Elements("element").ToArray();
                            if (int.TryParse(elements[0].Attribute("angle").Value, out int oldAngle))
                            {
                                int diffAngle = angle - oldAngle;
                                elements[0].Attribute("angle").Value = angle.ToString();

                                for (int i = 1; i < elements.Length; i++)
                                {
                                    var element = elements[i];
                                    if (int.TryParse(element.Attribute("angle").Value, out int oldArrownAngle))
                                    {
                                        int newArrownAngle = oldArrownAngle + diffAngle;
                                        element.Attribute("angle").Value = newArrownAngle.ToString();
                                    }
                                }
                            }
                        }
                        using (StreamWriter writer = new StreamWriter(xmlFilename))
                        {
                            doc.Save(writer);
                            success = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (Message)
                {
                    ShowMessage(ErrorMessage + ": " + e.Message);
                }
            }

            if (Message)
            {
                if (success)
                {
                    ShowMessage(SuccessMessage);
                }
                else
                {
                    ShowMessage(ErrorMessage);
                }
            }
        }
    }

    private void ShowMessage(string message)
    {
        Message.color = new Color(Message.color.r, Message.color.g, Message.color.b, 0.0f);
        Message.text = message;
        StartCoroutine(HideMessage());
    }
    private IEnumerator HideMessage()
    {
        float time = 0.0f;

        while (time < 1.0f)
        {
            time += Time.deltaTime * HiddingSpeed;
            Message.color = new Color(Message.color.r,
                                      Message.color.g,
                                      Message.color.b,
                                      Mathf.Lerp(1, 0, time));

            yield return new WaitForEndOfFrame();
        }
    }
}
