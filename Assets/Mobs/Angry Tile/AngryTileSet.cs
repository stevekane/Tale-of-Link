using System.Threading.Tasks;
using UnityEngine;

public class AngryTileSet : TaskRunnerComponent {
  public Timeval AngerPeriod = Timeval.FromMillis(500);

  AngryTile[] AngryTiles;

  void Start() {
    AngryTiles = new AngryTile[transform.childCount];
    for (var i = 0; i < AngryTiles.Length; i++) {
      AngryTiles[i] = transform.GetChild(i).GetComponent<AngryTile>();
    }
  }

  [ContextMenu("Anger")]
  public void Anger() {
    StartTask(Run);
  }

  int CellToArrayIndex(int x, int y, int w) => y * w + x;

  async Task Run(TaskScope scope) {
    const int HEIGHT = 4;
    const int WIDTH = 4;

    var x0 = 0;
    for (var y = 0; y < HEIGHT; y++) {
      for (var x = x0; x < WIDTH; x+=2) {
        var i = CellToArrayIndex(x,y,WIDTH);
        var angryTile = AngryTiles[i];
        angryTile.Anger();
      }
      x0 = x0 == 0 ? 1 : 0;
      await scope.Delay(AngerPeriod);
    }
    for (var y = HEIGHT-1; y >= 0; y--) {
      for (var x = x0; x < WIDTH; x+=2) {
        var i = CellToArrayIndex(x,y,WIDTH);
        var angryTile = AngryTiles[i];
        angryTile.Anger();
      }
      x0 = x0 == 0 ? 1 : 0;
      await scope.Delay(AngerPeriod);
    }
  }
}