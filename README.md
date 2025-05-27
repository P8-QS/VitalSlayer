<div align="center">
  
![VitalSlayer](/Assets/Artwork/icon.png)

</div>
<h1 align="center">
  <span title="Quantified Self Crawler">VitalSlayer</span>
</h1>
<div align="center">

![GitHub Downloads (all assets, all releases)](https://img.shields.io/github/downloads/P8-QS/VitalSlayer/total)
![GitHub contributors](https://img.shields.io/github/contributors/P8-QS/VitalSlayer)
![GitHub branch check runs](https://img.shields.io/github/check-runs/P8-QS/VitalSlayer/main)
![GitHub last commit](https://img.shields.io/github/last-commit/P8-QS/VitalSlayer)
![GitHub Release](https://img.shields.io/github/v/release/P8-QS/VitalSlayer)

</div>

## What is VitalSlayer?

**VitalSlayer** is a mobile dungeon crawling game that transforms your personal health data into engaging gameplay. By syncing with Health Connect, the game generates a new dungeon each day based on your:

**QS-Crawler** is a mobile dungeon crawling game that transforms your personal health data into engaging gameplay. By syncing with Health Connect, the game generates a new dungeon each day based on your:

- 👣 **Steps**
- 💤 **Sleep quality**
- ❤️ **Heart rate variability**
- 🔋 **Active calories**
- 📱 **Screen time**
- 🫁 **VO₂ Max**

Your daily habits influence your in-game stats, environment, and enemies — encouraging a fun, gamified loop of real-world self-improvement.

<div align="center">
  <table>
    <tr>
      <td><img src="Assets/Artwork/Gifs/menu00.gif" alt="Menu Navigation" width="245"/></td>
      <td><img src="Assets/Artwork/Gifs/gameplay02.gif" alt="Dungeon Gameplay" width="245"/></td>
      <td><img src="Assets/Artwork/Gifs/gameplay01.gif" alt="Dungeon Gameplay" width="245"/></td>
    </tr>
    <tr>
      <td align="center"><em>Menu, Effects and Perks</em></td>
      <td align="center"><em>Gameplay 1</em></td>
      <td align="center"><em>Gameplay 2</em></td>
    </tr>
  </table>
</div>

---

## 🧩Features

- **Daily Dungeon Generation**  
  Each day, you’ll face a new dungeon experience shaped by your previous day’s health metrics.

- **Data-Driven Gameplay**  
  Your sleep, heart rate, steps, VO₂ Max, and more affect enemies, dungeon size and type, and your hero’s stats.

- **Health Awareness Through Play**  
  Understand your own wellness trends through game feedback and daily summaries.

- **Dynamic Dungeon Crawling**  
  Battle through procedurally generated levels with classic dungeon-crawler mechanics.

- **Health Platform Integration**  
  Currently supports [**Health Connect**](https://health.google/health-connect-android/) (Android), enabling compatibility with a wide range of health apps including Google Fit, Samsung Health, Fitbit, and more.

---

## 🚀 Getting Started

### 📱 Installation

1. **Download the latest release** from the [Releases page](https://github.com/P8-QS/VitalSlayer/releases).
2. Install the APK on your Android device (you may need to allow installs from unknown sources).
3. Grant necessary permissions for accessing Health Connect data and usage data.
4. Start crawling dungeons!

> ℹ️ _VitalSlayer currently supports Android devices only. iOS support is planned for future releases._

---

### 🤝 To contribute:

1. **Download and install [Unity Hub](https://unity.com/download)**.
2. **Fork the repository** on GitHub.
3. Clone your fork locally and open it in Unity.
4. Create your feature branch:  
   `git checkout -b feature/myFeature`
5. Make your changes within Unity (add scenes, scripts, gameplay changes, etc).
6. Commit your changes:  
   `git commit -m 'Add my feature'`
7. Push to your fork:  
   `git push origin feature/myFeature`
8. Open a Pull Request on the original repo.

---

## 🧩 How It Works

Each day, your dungeon is shaped by your real-world habits. For example:

| Metric              | In-Game Effect                                                                                                             |
| ------------------- | -------------------------------------------------------------------------------------------------------------------------- |
| **Steps**           | Steps affect map size, more steps means larger map                                                                         |
| **Sleep**           | Poor sleep causes hallucinations, making the player see enemies that aren't real                                           |
| **Heart Rate/HRV**  | Affects the variety of enemy levels that spawn — higher HRV means greater variety                                          |
| **Active Calories** | Determines whether doors open freely or require clearing enemies first                                                     |
| **Screen Time**     | Affects map visibility — poor screen time adds fog (only current/visited rooms visible), good screen time reveals full map |
| **VO₂ Max**         | High VO₂ Max grants immunity to slow effects; low VO₂ Max leaves player vulnerable                                         |

These mechanics not only reward healthy habits and encourage daily reflection, but also help players better understand their health data — such as the impact of heart rate variability on overall well-being.

---

## 📈 Health + Game Feedback

At the start of each dungeon run, you’ll receive:

- A **summary screen** highlighting which health metrics will influence the game
- Short tips or insights encouraging habit improvement
  An in-game NPC offers general health tips, explains metrics like HRV or VO₂ Max in simple terms, and gives advice on how to improve them through daily habits.

---

## 🎓 Academic Context

**VitalSlayer** was developed as part of a research project at **Aalborg University**,  
investigating how gamified health data can promote reflection, understanding, and behavior change.

Read more in the accompanying [paper](...) (Paper not finished yet).

---

## 📝 User manual

Here is a [comprehensive user manual](https://github.com/user-attachments/assets/6dbcd4d9-5bd5-4a42-9ed2-cc77805afdce) to guide new players through installation and permissions setup.

<div align="center">
  
<img src="https://github.com/user-attachments/assets/6dbcd4d9-5bd5-4a42-9ed2-cc77805afdce" alt="User Manual Preview" width="600"/> 
  
</div>

---

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙋 FAQ

**Q: Do I need to manually input my health data?**  
A: Nope! As long as you’re synced with Health Connect, your metrics will automatically inform the game.

**Q: What if I have low activity one day?**  
A: The dungeon reflects it, but you can still play. Some days might be tougher — just like in life!

**Q: Can I compare my dungeon with friends?**  
A: Multiplayer or social dungeon sharing is on the roadmap!

---

## 🔮 Roadmap

- [ ] iOS support via Apple HealthKit
- [ ] Multiplayer dungeon challenges
- [ ] Customizable avatars and progression
- [ ] AI-powered health insights from dungeon data
- [ ] Leaderboards

---

[Back to top](#top)
