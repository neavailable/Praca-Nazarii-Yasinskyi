using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using MainScene.BoostPad;
using MainScene.Chaser;
using MainScene.Coin;
using MainScene.Finish.FinishTrigger;
using MainScene.Heal;
using MainScene.Obstacles;
using MainScene.StartPoints;
using MainScene.UI.Timer;
using MainScene.Wall.WallsMarker;

namespace BallFinisher.Editor
{
    // Draws each level from above as an SVG plan with a legend, for the thesis: walls, start, finish,
    // coins, enemies and every placed object, in the same positions as in the scene.
    public static partial class FeatureSetup
    {
        private const string PlansFolder = "Docs/Level Plans";
        private const float PlanScale = 8.0f, PlanMargin = 30.0f, LegendWidth = 290.0f;


        private static void ExportLevelPlan(string levelName, LevelLayout layout)
        {
            List<Vector3[]> walls = FindInScene<WallMarker>()
                .Select(wall => wall.GetComponentInChildren<BoxCollider>())
                .Where(collider => collider != null)
                .Select(BoxCorners)
                .ToList();
            if (walls.Count == 0) return;

            float minX = walls.SelectMany(corners => corners).Min(point => point.x);
            float maxX = walls.SelectMany(corners => corners).Max(point => point.x);
            float minZ = walls.SelectMany(corners => corners).Min(point => point.z);
            float maxZ = walls.SelectMany(corners => corners).Max(point => point.z);

            float mapWidth  = (maxX - minX) * PlanScale + 2.0f * PlanMargin;
            float height    = Mathf.Max((maxZ - minZ) * PlanScale + 2.0f * PlanMargin + 40.0f, 560.0f);
            PlanCanvas plan = new (minX, maxZ, mapWidth + LegendWidth, height);

            plan.Rectangle(0, 0, mapWidth + LegendWidth, height, Hex(Color.Lerp(layout.Theme.Ground, Color.white, 0.55f)), "none");
            plan.Text(PlanMargin, 30, layout.Title + " (" + layout.Theme.Name + ")", 20, "#222", true);

            foreach (Vector3[] corners in walls) plan.Polygon(corners, Hex(layout.Theme.Wall), "#333", 0.5f);

            // Objects placed by the setup.
            foreach (MovingWall wall in FindInScene<MovingWall>())
            {
                Vector3 axis = wall.transform.right;
                plan.Line(wall.transform.position - axis * MovingWallReach, wall.transform.position + axis * MovingWallReach, "#6b4a2a", 1.5f, true);
                plan.OrientedRectangle(wall.transform.position, axis, MovingWallLength, 0.5f, Hex(layout.Theme.Wall), "#333");
            }
            foreach (Spinner spinner in FindInScene<Spinner>())
            {
                plan.Circle(spinner.transform.position, SpinnerRadius, "none", "#555", 1.2f, true);
                Vector3 axis = spinner.transform.right;
                plan.Line(spinner.transform.position - axis * (SpinnerRadius - 0.1f), spinner.transform.position + axis * (SpinnerRadius - 0.1f), "#555", 3.0f, false);
            }
            foreach (BoostPadTrigger pad in FindInScene<BoostPadTrigger>())
            {
                plan.OrientedRectangle(pad.transform.position, pad.transform.right, 2.5f, 4.0f, Hex(BoostPadColor), "#7a3d00");
                plan.Arrow(pad.transform.position - pad.transform.forward * 1.5f, pad.transform.position + pad.transform.forward * 1.5f, "#fff");
            }
            foreach (Bumper bumper in FindInScene<Bumper>()) plan.Circle(bumper.transform.position, 0.8f, Hex(BumperColor), "#5a1050", 1.0f, false);
            foreach (HealPickup heal in FindInScene<HealPickup>()) plan.Cross(heal.transform.position, 0.6f, Hex(HealColor));
            foreach (TimePickup pickup in FindInScene<TimePickup>())
            {
                bool isPenalty = new SerializedObject(pickup).FindProperty("_isPenalty").boolValue;
                plan.Circle(pickup.transform.position, 0.6f, Hex(isPenalty ? TimeTrapColor : TimeCoinColor), "#fff", 1.0f, false);
                plan.CenteredText(pickup.transform.position, isPenalty ? "-" : "+", 11, "#fff");
            }

            // Coins, enemies, start and finish from the level design.
            foreach (CoinsStartPointMarker coin in FindInScene<CoinsStartPointMarker>()) plan.Circle(coin.transform.position, 0.35f, "#f2c200", "#8a6d00", 0.6f, false);
            foreach (EnemyStartPoint enemy in FindInScene<EnemyStartPoint>())
            {
                if (enemy.EnemyType == EnemyStartPoint.EnemyTypes.Cactus && enemy.transform.childCount >= 2)
                {
                    plan.Line(enemy.transform.GetChild(0).position, enemy.transform.GetChild(1).position, "#c62828", 1.2f, true);
                    plan.Triangle(enemy.transform.position, 0.7f, "#c62828");
                }
                else plan.Circle(enemy.transform.position, 0.55f, "#e53935", "#7f0000", 0.8f, false);
            }
            foreach (Chaser chaser in FindInScene<Chaser>()) plan.Diamond(chaser.transform.position, 0.8f, Hex(ChaserColor));

            Vector3 finish = FindInScene<FinishTrigger>().First().transform.position;
            plan.Square(finish, 1.0f, "#ffd400", "#333");
            plan.CenteredText(finish, "F", 11, "#333");
            Vector3 start = GetBallStart();
            plan.Circle(start, 1.0f, "#1e63d6", "#fff", 1.5f, false);
            plan.CenteredText(start, "S", 11, "#fff");

            DrawLegend(plan, mapWidth, layout);

            string folder = Path.Combine(Directory.GetParent(Application.dataPath).FullName, PlansFolder);
            Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder, levelName + ".svg"), plan.ToSvg());
            Debug.Log($"[Feature Setup] {levelName}: plan saved to {PlansFolder}/{levelName}.svg");
        }

        private static void DrawLegend(PlanCanvas plan, float left, LevelLayout layout)
        {
            int Count<T>() where T : Object => FindInScene<T>().Count();
            int cacti     = FindInScene<EnemyStartPoint>().Count(enemy => enemy.EnemyType == EnemyStartPoint.EnemyTypes.Cactus);
            int mushrooms = FindInScene<EnemyStartPoint>().Count() - cacti;
            int traps     = FindInScene<TimePickup>().Count(pickup => new SerializedObject(pickup).FindProperty("_isPenalty").boolValue);

            (string Label, int Count, System.Action<Vector3> Icon)[] rows =
            {
                ("Start",                    1,                                  p => plan.Circle(p, 1.0f, "#1e63d6", "#fff", 1.5f, false)),
                ("Finish",                   1,                                  p => plan.Square(p, 1.0f, "#ffd400", "#333")),
                ("Coin",                     Count<CoinsStartPointMarker>(),     p => plan.Circle(p, 0.5f, "#f2c200", "#8a6d00", 0.6f, false)),
                ("Mushroom (stands still)",  mushrooms,                          p => plan.Circle(p, 0.6f, "#e53935", "#7f0000", 0.8f, false)),
                ("Cactus (patrols)",         cacti,                              p => plan.Triangle(p, 0.8f, "#c62828")),
                ("Chaser (follows the ball)", Count<Chaser>(),                   p => plan.Diamond(p, 0.9f, Hex(ChaserColor))),
                ("Boost pad",                Count<BoostPadTrigger>(),           p => plan.Square(p, 0.9f, Hex(BoostPadColor), "#7a3d00")),
                ("Bumper",                   Count<Bumper>(),                    p => plan.Circle(p, 0.8f, Hex(BumperColor), "#5a1050", 1.0f, false)),
                ("Spinning bar",             Count<Spinner>(),                   p => plan.Circle(p, 0.9f, "none", "#555", 1.2f, true)),
                ("Moving wall",              Count<MovingWall>(),                p => plan.Square(p, 0.9f, Hex(layout.Theme.Wall), "#333")),
                ("Heal",                     Count<HealPickup>(),                p => plan.Cross(p, 0.8f, Hex(HealColor))),
                ("Time coin (+time)",        Count<TimePickup>() - traps,        p => plan.Circle(p, 0.7f, Hex(TimeCoinColor), "#fff", 1.0f, false)),
                ("Time trap (-time)",        traps,                              p => plan.Circle(p, 0.7f, Hex(TimeTrapColor), "#fff", 1.0f, false))
            };

            float x = left + 10.0f, y = 70.0f;
            plan.Rectangle(x - 6, y - 26, LegendWidth - 20, rows.Length * 30 + 40, "#ffffffcc", "#999");
            plan.Text(x + 4, y - 6, "Legend", 15, "#222", true);

            foreach ((string label, int count, System.Action<Vector3> icon) in rows)
            {
                if (count == 0) continue;

                y += 30.0f;
                icon(plan.ToWorld(x + 18, y - 5));
                plan.Text(x + 40, y, label + "  x" + count, 13, "#222", false);
            }
        }

        private static Vector3[] BoxCorners(BoxCollider box)
        {
            Vector3 half = box.size / 2.0f;
            return new []
            {
                box.transform.TransformPoint(box.center + new Vector3(-half.x, 0, -half.z)),
                box.transform.TransformPoint(box.center + new Vector3( half.x, 0, -half.z)),
                box.transform.TransformPoint(box.center + new Vector3( half.x, 0,  half.z)),
                box.transform.TransformPoint(box.center + new Vector3(-half.x, 0,  half.z))
            };
        }

        private static string Hex(Color color) => "#" + ColorUtility.ToHtmlStringRGB(color);


        // World X goes right and world Z goes up on the page.
        private class PlanCanvas
        {
            private readonly StringBuilder _svg = new ();
            private readonly float _minX, _maxZ, _width, _height;


            public PlanCanvas(float minX, float maxZ, float width, float height)
            {
                _minX   = minX;
                _maxZ   = maxZ;
                _width  = width;
                _height = height;
            }

            public string ToSvg()
            {
                return $"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{F(_width)}\" height=\"{F(_height)}\" " +
                       $"viewBox=\"0 0 {F(_width)} {F(_height)}\" font-family=\"Arial, Helvetica, sans-serif\">\n{_svg}</svg>\n";
            }

            public Vector3 ToWorld(float px, float py) => new (_minX + (px - PlanMargin) / PlanScale, 0, _maxZ - (py - PlanMargin - 40) / PlanScale);

            public void Rectangle(float x, float y, float width, float height, string fill, string stroke)
            {
                _svg.Append($"<rect x=\"{F(x)}\" y=\"{F(y)}\" width=\"{F(width)}\" height=\"{F(height)}\" fill=\"{fill}\" stroke=\"{stroke}\" rx=\"4\"/>\n");
            }

            public void Polygon(IEnumerable<Vector3> points, string fill, string stroke, float strokeWidth)
            {
                string list = string.Join(" ", points.Select(point => { Vector2 p = Map(point); return F(p.x) + "," + F(p.y); }));
                _svg.Append($"<polygon points=\"{list}\" fill=\"{fill}\" stroke=\"{stroke}\" stroke-width=\"{F(strokeWidth)}\"/>\n");
            }

            public void OrientedRectangle(Vector3 center, Vector3 right, float length, float width, string fill, string stroke)
            {
                Vector3 side = Vector3.Cross(Vector3.up, right).normalized;
                Vector3 r = right.normalized * length / 2.0f, s = side * width / 2.0f;
                Polygon(new [] { center - r - s, center + r - s, center + r + s, center - r + s }, fill, stroke, 0.8f);
            }

            public void Circle(Vector3 center, float radius, string fill, string stroke, float strokeWidth, bool dashed)
            {
                Vector2 p = Map(center);
                string dash = dashed ? " stroke-dasharray=\"4 3\"" : "";
                _svg.Append($"<circle cx=\"{F(p.x)}\" cy=\"{F(p.y)}\" r=\"{F(radius * PlanScale)}\" fill=\"{fill}\" stroke=\"{stroke}\" stroke-width=\"{F(strokeWidth)}\"{dash}/>\n");
            }

            public void Square(Vector3 center, float halfSize, string fill, string stroke)
            {
                Vector2 p = Map(center);
                float size = halfSize * PlanScale;
                _svg.Append($"<rect x=\"{F(p.x - size)}\" y=\"{F(p.y - size)}\" width=\"{F(size * 2)}\" height=\"{F(size * 2)}\" fill=\"{fill}\" stroke=\"{stroke}\"/>\n");
            }

            public void Triangle(Vector3 center, float size, string fill)
            {
                Polygon(new [] { center + Vector3.forward * size, center + new Vector3(size * 0.87f, 0, -size * 0.5f), center + new Vector3(-size * 0.87f, 0, -size * 0.5f) }, fill, "#5a0000", 0.8f);
            }

            public void Diamond(Vector3 center, float size, string fill)
            {
                Polygon(new [] { center + Vector3.forward * size, center + Vector3.right * size, center + Vector3.back * size, center + Vector3.left * size }, fill, "#1a0016", 0.8f);
            }

            public void Cross(Vector3 center, float size, string fill)
            {
                OrientedRectangle(center, Vector3.right,   size * 2.0f, size * 0.7f, fill, "none");
                OrientedRectangle(center, Vector3.forward, size * 2.0f, size * 0.7f, fill, "none");
            }

            public void Line(Vector3 from, Vector3 to, string stroke, float strokeWidth, bool dashed)
            {
                Vector2 a = Map(from), b = Map(to);
                string dash = dashed ? " stroke-dasharray=\"5 4\"" : "";
                _svg.Append($"<line x1=\"{F(a.x)}\" y1=\"{F(a.y)}\" x2=\"{F(b.x)}\" y2=\"{F(b.y)}\" stroke=\"{stroke}\" stroke-width=\"{F(strokeWidth)}\"{dash}/>\n");
            }

            public void Arrow(Vector3 from, Vector3 to, string stroke)
            {
                Line(from, to, stroke, 1.6f, false);
                Vector3 direction = (to - from).normalized;
                Vector3 side = Vector3.Cross(Vector3.up, direction);
                Line(to, to - direction * 0.8f + side * 0.6f, stroke, 1.6f, false);
                Line(to, to - direction * 0.8f - side * 0.6f, stroke, 1.6f, false);
            }

            public void Text(float x, float y, string text, int size, string fill, bool bold)
            {
                string weight = bold ? " font-weight=\"bold\"" : "";
                _svg.Append($"<text x=\"{F(x)}\" y=\"{F(y)}\" font-size=\"{size}\" fill=\"{fill}\"{weight}>{Escape(text)}</text>\n");
            }

            public void CenteredText(Vector3 center, string text, int size, string fill)
            {
                Vector2 p = Map(center);
                _svg.Append($"<text x=\"{F(p.x)}\" y=\"{F(p.y + size * 0.35f)}\" font-size=\"{size}\" fill=\"{fill}\" text-anchor=\"middle\" font-weight=\"bold\">{Escape(text)}</text>\n");
            }


            private Vector2 Map(Vector3 world)
            {
                return new Vector2
                (
                    PlanMargin + (world.x - _minX) * PlanScale,
                    PlanMargin + 40.0f + (_maxZ - world.z) * PlanScale
                );
            }

            private static string F(float value) => value.ToString("0.#", CultureInfo.InvariantCulture);

            private static string Escape(string text) => text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }
    }
}
