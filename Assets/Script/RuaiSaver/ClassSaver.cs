using System.Collections;
using Newtonsoft.Json;

interface ClassSaver
{
    void LoadClass(string content);
    string SaveClass();
    string GetID();
}
