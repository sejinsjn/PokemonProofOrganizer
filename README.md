# PokemonProofOrganizer

Program to organize my proof (videos) for Pokemon events. It can compress, rename videos, create folder and add a trade history as .txt file.

## Installation

Use the package manager [Chocolatey](https://chocolatey.org/install) to install ffmpeg. You can skip this, if video compressing isn't needed.

```bash
choco install ffmpeg
```

## Usage

![image](https://github.com/sejinsjn/PokemonProofOrganizer/assets/76414770/4b5dc616-1c40-4e5c-bc7d-3d0c35188143)

1. Click on Browse to choose your files
2. [Optional] Input a decimal number. If nothing added, it will start from zero.
3. [Optional] Choose your prefix. The default is 12.
4. Opt in what you want to do with the files. 
5. Add to Queue
6. Start

Ternary Number calculates the ternary number of the decimal you have inputted and renames the files with the ternary number (only if "Rename" was opted in). Its maximum is 4 digits.
Prefix adds a prefix to the ternary number.
Create Folder creates a folder with the name of the file.
Add Trade History adds a text file. Content is optional and can be edited through the Edit button.
Compress simply compresses the video.

## License

[MIT](https://choosealicense.com/licenses/mit/)
