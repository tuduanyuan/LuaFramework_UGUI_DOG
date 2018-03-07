
local transform;
local gameObject;

Cube = {}
local this = Cube;

function Cube.Awake(obj)
    gameObject = obj;
    transform = obj.transform;
    FixedUpdateBeat:Add(this.FixedUpdate);
end
function Cube.Start()
    log("Cube开始运行了");
    --我要他一直运行下去
    
end
local hello = false;
function Cube.FixedUpdate()
    if (hello == true) then
        log("FixedUpdate");
    end
    local eAngle = transform.eulerAngles;
    transform:Rotate(Vector3.up,1);
end

function Cube.OnDestroy()
    log("Cube删除了");
    hello = true;
    FixedUpdateBeat:Remove(this.FixedUpdate);
end