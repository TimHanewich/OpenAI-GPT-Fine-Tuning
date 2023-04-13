## Training Data
I've prepared training data in the format that is required by OpenAI/Azure OpenAI (.jsonl files). You can download them from the table below:
|Description|Size|Download|
|-|-|-|
|Every SpongeBob SquarePants episode, 10,000 character prompt limit|449 MB|[here](https://timhmsft.blob.core.windows.net/downloadable/spongebob_training_f161f36fb6254a709ff38019589c88cf.jsonl?sp=r&st=2023-04-12T16:02:42Z&se=2999-04-13T00:02:42Z&sv=2021-12-02&sr=b&sig=twlNBZH0jjtmRCWeiMBckY%2Fkqknnsa1%2BvoxPbScf8zE%3D)|
|Every SpongeBob SquarePants episode, 10,000 character prompt limit, but **cleansed of errors**|448 MB|[here](https://timhmsft.blob.core.windows.net/downloadable/spongebob_training_eaefeeec703e488abee1f79ce413c651.jsonl?sp=r&st=2023-04-12T16:51:54Z&se=2999-04-13T00:51:54Z&sv=2021-12-02&sr=b&sig=WmXqrOHG6VT72XI1hMk8UAaSpMDTT9tVm9ln9EMOr6o%3D)|
|The same file as above, but trimmed the bottom portion of it to limit the file size|283 MB|[here](https://timhmsft.blob.core.windows.net/downloadable/spongebob_training_d098454a128a4301bd2762613e9ee6f6.jsonl?sp=r&st=2023-04-12T20:56:01Z&se=2999-04-13T04:56:01Z&sv=2021-12-02&sr=b&sig=V%2F%2BinePNI%2FvRjuVXgRU3yudyhsQp5bUJoHwzBZGFW%2B0%3D)|
|Same as above, but trimmed bottom portion to limit file size|145 MB|[here](https://timhmsft.blob.core.windows.net/downloadable/spongebob_training_042642cb31ac435cb895868d38d354b3.jsonl?sp=r&st=2023-04-12T21:12:10Z&se=2999-04-13T05:12:10Z&sv=2021-12-02&sr=b&sig=nsMSXsfNYFIrViWOxQ4Fec84o7r5ZIa2SGjVTkSeA2g%3D)|
|Same as above, but trimmed bottom portion to limit to 15,000 lines|95 MB|[here](https://timhmsft.blob.core.windows.net/downloadable/spongebob_training_ecad88d.jsonl?sp=r&st=2023-04-12T21:29:50Z&se=2999-04-13T05:29:50Z&sv=2021-12-02&sr=b&sig=qTrnvlI6%2FlzwNGuLDxsveV79EwkqI2%2FZc%2FVExfQeHas%3D)|

## Gilligan's Island
Gilligans Island Scripts: http://www.gilligansisle.com/scripts.html

Needs work:
- http://www.gilligansisle.com/scripts/script37.html
- http://www.gilligansisle.com/scripts/script55.html
- http://www.gilligansisle.com/scripts/script71.html
- http://www.gilligansisle.com/scripts/script91.html
- http://www.gilligansisle.com/scripts/script95.html
- Every one that uses the HTML

## Structure for Text Messages Data Container
Folder structure:
```
- conversations
    - 9418473894.json
    - 9417773842.json
```

Each JSON file in the `conversations` folder contains the conversation like this:
```
[
    {
        "speaker": 0,
        "body": "Hi, Tim, how are you?"
    },
    {
        "speaker": 1,
        "body": "I am good, thank you. How are you?"
    }
]
```
- `speaker` 0 is the person speaking with Tim, `speaker` 1 is Tim's response.

## Spongebob Scripts
- Scripts can be found [here](https://spongebob.fandom.com/wiki/List_of_transcripts).