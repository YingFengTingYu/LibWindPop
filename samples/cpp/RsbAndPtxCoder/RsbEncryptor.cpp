#include "RsbEncryptor.h"

struct CompiledMap {
    struct Node {
        unsigned int nodeData;

        unsigned char ch() {
            return nodeData;
        }

        unsigned int alt() {
            return nodeData >> 8;
        }

        void Encrypt() {
            unsigned int address = nodeData >> 8;
            unsigned char chr = nodeData;
            nodeData = chr | (address << 9);
        }
    };

    Node* nodePtr;
    unsigned int nodeCount;

    CompiledMap()
    {
        nodePtr = NULL;
        nodeCount = 0;
    }

    void Init(void* dataPtr, unsigned int dataSize) {
        nodePtr = (Node*)dataPtr;
        nodeCount = dataSize >> 2;
    }

    bool Initialized() {
        return nodePtr != 0;
    }

    Node* Find(const unsigned char* findStr) {
        if (nodeCount) {
            return FindFast(findStr);
        }
        return 0;
    }

    Node* FindFast(const unsigned char* findStr) {
        Node* startPtr;
        unsigned char upperChar;
        Node* fNodePtr;

        fNodePtr = nodePtr;
        while (fNodePtr)
        {
            upperChar = toupper(*findStr);
            if (fNodePtr->ch() == upperChar)
            {
                if (!fNodePtr->ch())
                {
                    return fNodePtr + 1;
                }
                ++fNodePtr;
                ++findStr;
            }
            else
            {
                if (fNodePtr->ch() > (int)upperChar)
                {
                    return 0;
                }
                if (!fNodePtr->alt())
                {
                    return 0;
                }
                startPtr = nodePtr;
                fNodePtr = &startPtr[fNodePtr->alt()];
            }
        }
        return 0;
    }

private:
    void EncryptNextInfo(unsigned int offset)
    {
        Node* pointer_32_bits = nodePtr + offset;
        unsigned int next_offset;
        while (offset < nodeCount && pointer_32_bits->ch())
        {
            next_offset = pointer_32_bits->alt();
            if (next_offset != 0u)
            {
                EncryptNextInfo(next_offset);
                pointer_32_bits->Encrypt();
            }
            offset++;
            pointer_32_bits++;
        }
    }

public:
    void Encrypt() {
        EncryptNextInfo(0u);
    }
};

void encryptRsb(string rsbPath) {
    fstream rsbStream(rsbPath, ios::in | ios::out | ios::binary);
    unsigned int headerSize = 0u;
    rsbStream.seekg(12, ios::beg);
    rsbStream.read((char*)&headerSize, 4);
    void* rsbHeaderMemory = malloc(headerSize);
    rsbStream.seekg(0, ios::beg);
    rsbStream.read((char*)rsbHeaderMemory, headerSize);
    unsigned int* rsbInfoPtr = (unsigned int*)rsbHeaderMemory;
    CompiledMap map;
    map.Init((unsigned char*)rsbHeaderMemory + rsbInfoPtr[5], rsbInfoPtr[4]);
    map.Encrypt();
    map.Init((unsigned char*)rsbHeaderMemory + rsbInfoPtr[9], rsbInfoPtr[8]);
    map.Encrypt();
    map.Init((unsigned char*)rsbHeaderMemory + rsbInfoPtr[17], rsbInfoPtr[16]);
    map.Encrypt();
    rsbStream.seekp(0, ios::beg);
    rsbStream.write((char*)rsbHeaderMemory, headerSize);
    free(rsbHeaderMemory);
    rsbStream.close();
}

void encryptRsg(string rsgPath) {
    fstream rsgStream(rsgPath, ios::in | ios::out | ios::binary);
    unsigned int magic = 0u;
    rsgStream.read((char*)&magic, 4);
    if (magic == 0x72736770u) {
        rsgStream.seekg(0x48, ios::beg);
        unsigned int mapSize = 0u, mapOffset = 0u;
        rsgStream.read((char*)&mapSize, 4);
        rsgStream.read((char*)&mapOffset, 4);
        // alloc memory
        void* rsgMapMemory = malloc(mapSize);
        rsgStream.seekg(mapOffset, ios::beg);
        rsgStream.read((char*)rsgMapMemory, mapSize);
        // create map
        CompiledMap map;
        map.Init(rsgMapMemory, mapSize);
        map.Encrypt();
        // write
        magic = 0x72736763u;
        rsgStream.seekp(0, ios::beg);
        rsgStream.write((char*)&magic, 4);
        rsgStream.seekp(mapOffset, ios::beg);
        rsgStream.write((char*)rsgMapMemory, mapSize);
        // free memory
        free(rsgMapMemory);
    }
    rsgStream.close();
}

void getRsgs(string path, vector<string>& files)
{
    long long hFile = 0;
    _finddata_t fileinfo;
    string p;
    if ((hFile = _findfirst(p.assign(path).append("\\*.rsg").c_str(), &fileinfo)) != -1) {
        do {
            files.push_back(p.assign(path).append("\\").append(fileinfo.name));
        } while (_findnext(hFile, &fileinfo) == 0);
        _findclose(hFile);
    }
}

extern "C" {
	void RsbEncryptorOnStartBuild(const char* unpackPath) {
		// Encrypt all rsg
        vector<string> rsgs;
        string rgtempPath(unpackPath);
        rgtempPath = rgtempPath.append("\\rgtemp");
        getRsgs(rgtempPath, rsgs);
        for (vector<string>::iterator it = rsgs.begin(); it != rsgs.end(); it++)
        {
            encryptRsg(*it);
        }
	}
	void RsbEncryptorOnEndBuild(const char* rsbPath) {
		// Encrypt rsb
        encryptRsb(rsbPath);
	}
	void RsbEncryptorOnAdd(const char* unpackPath) {
		// Nothing to do...
	}
}
