Gilligans Island Scripts: http://www.gilligansisle.com/scripts.html

Needs work:
- http://www.gilligansisle.com/scripts/script37.html
- http://www.gilligansisle.com/scripts/script55.html
- http://www.gilligansisle.com/scripts/script71.html
- http://www.gilligansisle.com/scripts/script91.html
- http://www.gilligansisle.com/scripts/script95.html
- Everyone that uses the HTML

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