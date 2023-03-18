# KakaoTalkAutoLogin

여러 PC에서 카카오톡 자동 로그인


https://user-images.githubusercontent.com/15166740/225289222-b16525f1-9239-4537-9c79-9414aeaa191d.mp4


## credentials.json

```json
{
  "kakaoAccount" : "exjang0@gmail.com",
  "password" : "echo MyPasswordHere | openssl enc -aes-256-cbc -a -salt -pass pass:kamikami -iter 100"
}
키가 공개되어 있어서 암호화는 사실 아무 쓸모없어요
```

## 이게 안전한지 어떻게 알고 암호를 막 쳐요?

그러게요! 소스에서 빌드하세요!

(당연히 바이러스는 없습니다만, 윈도우 디펜더에 걸리네요. 나중에 많은 분들이 쓰게 되면 exe 서명을 추가하도록 할게요)


## PC가 켜지면 자동 실행되게 설정하는 방법

### 시작->실행-> shell:startup
![image](https://user-images.githubusercontent.com/15166740/225290613-5f575cb5-8316-405c-968b-13ef86f9d39f.png)
![image](https://user-images.githubusercontent.com/15166740/225290696-e46cf3de-774c-4f1a-97da-00fca9af2612.png)

