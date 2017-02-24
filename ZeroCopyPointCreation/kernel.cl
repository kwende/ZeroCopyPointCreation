typedef struct _Point3D 
{ 
    float X; 
    float Y; 
    float Z; 
} Point3D; 

kernel void ComputePoints(
    global read_only ushort* input,
    global write_only Point3D* output,
    global read_only float* M,
    global read_only float* b,
    int frameWidth)
{
    int x = get_global_id(0); 
    int y = get_global_id(1); 
    int i = y * frameWidth + x; 
    ushort z = input[i]; 

    output[i].X = x * M[0] + y * M[1] + z * M[2] + b[0]; 
    output[i].Y = x * M[3] + y * M[4] + z * M[5] + b[1]; 
    output[i].Z = x * M[6] + y * M[7] + z * M[8] + b[2];
}
