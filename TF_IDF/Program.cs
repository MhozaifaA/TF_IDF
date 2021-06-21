using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

bool IsInText = true;
string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) , "Output.txt");
FileStream ostrm; StreamWriter writer; TextWriter oldOut = Console.Out; try { ostrm = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write); writer = new StreamWriter(ostrm); } catch (Exception e) { Console.WriteLine("Cannot open Redirect.txt for writing"); Console.WriteLine(e.Message); return; } if (IsInText) Console.SetOut(writer);

Console.WriteLine("Hello World!");

//Sentences
var Sentences = new List<string>{ 
    "حضر محاضرة في اللغات"  ,
    "حضر اللغات في اللغات الطبيعية",
};

//Set words
var Words = Sentences.Aggregate(new HashSet<string>(),
            (wordsSet, nextSentences) => {
                nextSentences.Trim().Split(" ").Select(w => w.Trim()).ToList().ForEach(w => wordsSet.Add(w));
                return wordsSet; });

//Split Sentences
var SentencesEx = Sentences.ToDictionary(s => s, s => s.Trim().Split(" ").Select(w => w.Trim()));


//Calculate TF
var TFs = SentencesEx.Select(sentence =>  new KeyValuePair<string, Dictionary<string, double>>
        (sentence.Key, Words.Select(word => 
                    new KeyValuePair<string, double>(word, 
                                sentence.Value.Count(w => w.Equals(word)) / 
                                (double)sentence.Value.Count())).
                                            ToDictionary(w => w.Key, w => w.Value))).
                                                    ToDictionary(t=>t.Key,t=>t.Value);

//Print TFs
Console.WriteLine($"\t\t\t------------  TFs  ------------\n");
TFs.ToList().ForEach(tf=> {
    Console.WriteLine($"\t\t\t***********************\n");
    Console.WriteLine($"Sentence: {tf.Key}");
    Console.WriteLine($"_________________");
    tf.Value.ToList().ForEach(t=> Console.WriteLine($"Word: {t.Key} \t\t\t\t\t > TF: {t.Value}"));
    Console.WriteLine($"\t\t\t***********************\n");
});


//Calculate IDF
var IDFs = Words.Select(word => new KeyValuePair<string, double>
                        (word, Math.Log10(Sentences.Count /
                                        (double)SentencesEx.Where(s => s.Value.Contains(word)).Count()))).
                                                ToDictionary(w=>w.Key,w=>w.Value);



//Print IDFs
Console.WriteLine($"\t\t\t------------  IDFs  ------------\n");
IDFs.ToList().ForEach(idf=> Console.WriteLine($"Word: {idf.Key} \t\t\t\t\t > IDF: {idf.Value}"));
Console.WriteLine($"\t\t\t***********************\n");




//Print TFs×IDFs 
Console.WriteLine($"\t\t\t------------  TFs×IDFs  ------------\n");
SentencesEx.ToList().ForEach(sentence => {
    Console.WriteLine($"\t\t\t***********************\n");
    Console.WriteLine($"Sentence: {sentence.Key}");
    Console.WriteLine($"_________________");
    sentence.Value.ToList().ForEach(word => {
        Console.WriteLine($"Word: {word} \t\t\t\t\t > TF×IDF: {TFs[sentence.Key][word] * IDFs[word]}");
    });
});


//Done & Open
Console.SetOut(oldOut); writer.Close(); ostrm.Close(); Console.WriteLine("Done");
System.Diagnostics.Process.Start("Notepad.exe", path);
